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

        public CharacterService(ICharacterRepository repository, ICurrentUserService currentUserService)
        {
            _characterRepository = repository;
            _currentUserService = currentUserService;
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
    }
}
