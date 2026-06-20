using Ghoredin.Application.Campaigns;
using Ghoredin.Application.Users;
using Ghoredin.Domain.Characters;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Characters
{
    public class CharacterService: ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICampaignRepository _campaignRepository;

        public CharacterService(ICharacterRepository repository, ICurrentUserService currentUserService, ICampaignRepository campaignRepository)
        {
            _characterRepository = repository;
            _currentUserService = currentUserService;
            _campaignRepository = campaignRepository;
        }

        public async Task<CharacterDto> CreateAsync(CreateCharacterCommand command)
        {
            var character = new Character
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                GameSystemId = command.GameSystemId,
                SheetData = command.SheetData,
                OwnerUserId = _currentUserService.UserId
                    ?? throw new InvalidOperationException("Není přihlášený uživatel.")
            };

            await _characterRepository.AddAsync(character);

            await _characterRepository.SaveChangesAsync();

            return character.ToDto();
        }

        public async Task<List<CharacterDto>> GetMyCharactersAsync()
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var characters = await _characterRepository.GetByOwnerAsync(userId);

            return characters.Select(c => c.ToDto()).ToList();
        }

        public async Task<CharacterDto> GetByIdAsync(Guid id)
        {
            var character = await _characterRepository.GetByIdAsync(id);

            return character?.ToDto();
        }

        public async Task<CharacterDto> CreateInCampaignAsync(CreateCharacterInCampaignCommand command)
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var campaign = await _campaignRepository.GetByIdAsync(command.CampaignId)
                ?? throw new InvalidOperationException("Dobrodružství neexistuje.");

            var member = campaign.Members.FirstOrDefault(m => m.UserId == userId)
                ?? throw new InvalidOperationException("Nejsi členem tohoto dobrodružství.");

            if (member.CharacterId.HasValue)
                throw new InvalidOperationException("V tomto dobrodružství už máš postavu.");

            var character = new Character
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                GameSystemId = campaign.GameSystemId,
                SheetData = command.SheetData,
                OwnerUserId = userId,
                CampaignId = campaign.Id
            };

            await _characterRepository.AddAsync(character);

            member.CharacterId = character.Id;

            await _characterRepository.SaveChangesAsync();

            return character.ToDto();
        }
    }
}
