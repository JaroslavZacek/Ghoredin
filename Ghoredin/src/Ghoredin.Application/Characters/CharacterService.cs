using Ghoredin.Application.Campaigns;
using Ghoredin.Application.GameSystems;
using Ghoredin.Application.Users;
using Ghoredin.Domain.Campaigns;
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
        private readonly IGameSystemRegistry _gameSystemRegistry;

        public CharacterService(
            ICharacterRepository repository, 
            ICurrentUserService currentUserService, 
            ICampaignRepository campaignRepository,
            IGameSystemRegistry gameSystemsRegistry)
        {
            _characterRepository = repository;
            _currentUserService = currentUserService;
            _campaignRepository = campaignRepository;
            _gameSystemRegistry = gameSystemsRegistry;
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

        public async Task<List<CharacterDto>> GetCampaignCharactersAsync(Guid campaignId)
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var campaign = await _campaignRepository.GetByIdAsync(campaignId)
                ?? throw new InvalidOperationException("Dobrodružství neexistuje.");

            if (campaign.Members.All(m => m.UserId != userId))
                throw new InvalidOperationException("Nejsi členem tohoto dobrodružství.");

            var characters = await _characterRepository.GetByCampaignAsync(campaignId);

            return characters.Select(c => c.ToDto()).ToList();
        }


        #region Tvorba postavy

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

            var system = _gameSystemRegistry.Get(campaign.GameSystemId);
            var method = GetCreationMethod(campaign);

            if (method == "Roll")
                throw new InvalidOperationException("Pro toto dobrodružství se postava tvoří házením - použij tvorbu s hody.");

            var sheetData = command.SheetData ?? new Dictionary<string, object>();

            var abilities = ExtractAbilities(sheetData);
            var validation = system.ValidateAbilityScores(abilities, method);

            if (!validation.IsValid)
                throw new InvalidOperationException("Neplatné rozdělení atributů:" + string.Join(" ", validation.Errors));

            sheetData["hitPoints"] = new Dictionary<string, object> { ["current"] = 10, ["max"] = 10 };
            sheetData["creationComplete"] = true;

            var character = new Character
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                GameSystemId = campaign.GameSystemId,
                SheetData = sheetData,
                OwnerUserId = userId,
                CampaignId = campaign.Id
            };

            await _characterRepository.AddAsync(character);

            member.CharacterId = character.Id;

            await _characterRepository.SaveChangesAsync();

            return character.ToDto();
        }

        public async Task<CharacterDto> StartRolledCharacterAsync(Guid campaignId, string name)
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var campaign = await _campaignRepository.GetByIdAsync(campaignId)
                ?? throw new InvalidOperationException("Dobrodružství neexistuje.");

            var member = campaign.Members.FirstOrDefault(m => m.UserId == userId)
                ?? throw new InvalidOperationException("Nejsi členem tohoto dobrodružství.");

            if (member.CharacterId.HasValue)
                throw new InvalidOperationException("V tomto dobrodružství už máš postavu.");

            var method = GetCreationMethod(campaign);

            if (method != "Roll")
                throw new InvalidOperationException("Toto dobrodružství používá jinou metodu tvorby postav.");

            var abilities = new Dictionary<string, object>
            {
                ["strength"] = 0,
                ["dexterity"] = 0,
                ["constitution"] = 0,
                ["intelligence"] = 0,
                ["wisdom"] = 0,
                ["charisma"] = 0
            };

            var sheetData = new Dictionary<string, object>
            {
                ["abilities"] = abilities,
                ["creationComplete"] = false
            };

            var character = new Character
            {
                Id = Guid.NewGuid(),
                Name = name,
                GameSystemId = campaign.GameSystemId,
                SheetData = sheetData,
                OwnerUserId = userId,
                CampaignId = campaign.Id
            };

            await _characterRepository.AddAsync(character);

            member.CharacterId = character.Id;

            await _characterRepository.SaveChangesAsync();

            return character.ToDto();

        }


        // --------------------------------------------------------------------------------------
        // ----------------------------- Private methods ----------------------------------------
        // --------------------------------------------------------------------------------------

        private static string GetCreationMethod(Campaign campaign)
        {
            return campaign.GameSystemSettings.TryGetValue("characterCreation", out var m)
                ? m?.ToString() ?? "PointBuy"
                : "PointBuy";
        }

        private static Dictionary<string, object> ExtractAbilities(Dictionary<string, object> sheetData)
        {
            if (sheetData.TryGetValue("abilities", out var obj) && obj is Dictionary<string, object> dict)
                return dict;

            if (obj is System.Text.Json.JsonElement el && el.ValueKind == System.Text.Json.JsonValueKind.Object)
            {
                var result = new Dictionary<string, object>();

                foreach (var prop in el.EnumerateObject())
                {
                    result[prop.Name] = prop.Value;
                }

                return result;
            }

            return new Dictionary<string, object>();
        }

        #endregion


    }
}
