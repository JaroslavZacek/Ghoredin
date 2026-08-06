using Ghoredin.Application.Campaigns;
using Ghoredin.Application.GameSystems;
using Ghoredin.Application.Users;
using Ghoredin.Domain.Campaigns;
using Ghoredin.Domain.Characters;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace Ghoredin.Application.Characters
{
    public class CharacterService: ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IGameSystemRegistry _gameSystemRegistry;
        private readonly ICampaignAuthorizationService _campaignAuthorizationService;

        public CharacterService(
            ICharacterRepository repository, 
            ICurrentUserService currentUserService, 
            ICampaignRepository campaignRepository,
            IGameSystemRegistry gameSystemsRegistry,
            ICampaignAuthorizationService campaignAuthorizationService)
        {
            _characterRepository = repository;
            _currentUserService = currentUserService;
            _campaignRepository = campaignRepository;
            _gameSystemRegistry = gameSystemsRegistry;
            _campaignAuthorizationService = campaignAuthorizationService;
        }

        public async Task<List<CharacterDto>> GetMyCharactersAsync()
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var characters = await _characterRepository.GetByOwnerAsync(userId);

            var campaignIds = characters
                .Where(c => c.CampaignId.HasValue)
                .Select(c => c.CampaignId!.Value)
                .Distinct();

            var campaignNames = new Dictionary<Guid, string>();

            foreach (var campaignId in campaignIds)
            {
                var campaign = await _campaignRepository.GetByIdAsync(campaignId);
                if (campaign is not null)
                    campaignNames[campaignId] = campaign.Name;
            }

            return characters
                .Select(c => c.ToDto(c.CampaignId.HasValue ? campaignNames.GetValueOrDefault(c.CampaignId.Value) : null))
                .ToList();
        }

        public async Task<CharacterDto> GetByIdAsync(Guid id)
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var character = await _characterRepository.GetByIdAsync(id);

            if (character is null)
                return null;

            if (character.OwnerUserId == userId)
                return character.ToDto();

            if (character.CampaignId.HasValue)
            {
                var campaign = await _campaignRepository.GetByIdAsync(character.CampaignId.Value);

                if (campaign is not null)
                {
                    var isGm = _campaignAuthorizationService.IsGameMaster(campaign, userId);
                    var visibleToAll = GetCharactersVisibleToAll(campaign);
                    var isMember = _campaignAuthorizationService.IsMember(campaign, userId);

                    if (isGm || (visibleToAll && isMember))
                        return character.ToDto();
                }
            }

            throw new InvalidOperationException("Nemáš oprávnění zobrazit tuto postavu.");
        }

        public async Task<List<CharacterDto>> GetCampaignCharactersAsync(Guid campaignId)
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var campaign = await _campaignRepository.GetByIdAsync(campaignId)
                ?? throw new InvalidOperationException("Dobrodružství neexistuje.");

            if (campaign.Members.All(m => m.UserId != userId || !m.IsActive))
                throw new InvalidOperationException("Nejsi členem tohoto dobrodružství.");

            var characters = await _characterRepository.GetByCampaignAsync(campaignId);

            return characters.Select(c => c.ToDto()).ToList();
        }

        public async Task DeleteAsync(Guid characterId)
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var character = await _characterRepository.GetByIdAsync(characterId)
                ?? throw new InvalidOperationException("Postava neexistuje.");

            if (!character.CampaignId.HasValue)
                throw new InvalidOperationException("Postava nepatří do žádného dobrodružství.");

            var campaign = await _campaignRepository.GetByIdAsync(character.CampaignId.Value)
                ?? throw new InvalidOperationException("Dobrodružství neexistuje.");

            if (!_campaignAuthorizationService.IsGameMaster(campaign, userId))
                throw new InvalidOperationException("Jen PJ může smazat postavu.");

            var member = campaign.Members.FirstOrDefault(m => m.CharacterId == character.Id);
            if (member is not null)
                member.CharacterId = null;

            await _characterRepository.DeleteAsync(character);
            await _characterRepository.SaveChangesAsync();
        }


        #region Tvorba postavy

        public async Task<CharacterDto> CreateInCampaignAsync(CreateCharacterInCampaignCommand command)
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var campaign = await _campaignRepository.GetByIdAsync(command.CampaignId)
                ?? throw new InvalidOperationException("Dobrodružství neexistuje.");

            var member = campaign.Members.FirstOrDefault(m => m.UserId == userId && m.IsActive)
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

            var member = campaign.Members.FirstOrDefault(m => m.UserId == userId && m.IsActive)
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

        public async Task<CharacterDto> RollAbilityAsync(Guid characterId, string abilityName)
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var character = await _characterRepository.GetByIdAsync(characterId)
                ?? throw new InvalidOperationException("Postava neexistuje.");

            if (character.OwnerUserId != userId)
                throw new InvalidOperationException("Tuto postavu nevlastníš.");

            // Jen dokud je postava v procesu tvorby
            var complete = character.SheetData.TryGetValue("creationComplete", out var c) && ToBool(c);
            if (complete)
                throw new InvalidOperationException("Postava je již dokončena.");

            var abilities = ExtractAbilities(character.SheetData);
            if (!abilities.ContainsKey(abilityName))
                throw new InvalidOperationException($"Neznámý atribut: '{abilityName}'.");

            var current = ToInt(abilities.GetValueOrDefault(abilityName)) ?? 0;
            if (current > 0)
                throw new InvalidOperationException($"Na atribut '{abilityName}' už jsi házel.");

            // Hod kostkou přes herní systém
            var system = _gameSystemRegistry.Get(character.GameSystemId);
            var rolled = system.RollSingleAbilityScore()
                ?? throw new InvalidOperationException("Tento systém nepodporuje naházení atributů.");

            abilities[abilityName] = rolled;
            character.SheetData["abilities"] = abilities;

            await _characterRepository.SaveChangesAsync();
            return character.ToDto();
        }

        public async Task<CharacterDto> CompleteRolledCharacterAsync(Guid characterId)
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var character = await _characterRepository.GetByIdAsync(characterId)
                ?? throw new InvalidOperationException("Postava neexistuje.");

            if (character.OwnerUserId != userId)
                throw new InvalidOperationException("Tuto postavu nevlastníš.");

            var abilities = ExtractAbilities(character.SheetData);

            foreach (var ability in abilities)
            {
                if ((ToInt(ability.Value) ?? 0) <= 0)
                    throw new InvalidOperationException($"Ještě jsi neházel na všechny atributy (chybí '{ability.Key}').");
            }

            var system = _gameSystemRegistry.Get(character.GameSystemId);
            var validation = system.ValidateAbilityScores(abilities, "Roll");
            if (!validation.IsValid)
                throw new InvalidOperationException("Neplatné atributy: " + string.Join(" ", validation.Errors));

            character.SheetData["hitPoints"] = new Dictionary<string, object> { ["current"] = 10, ["max"] = 10 };
            character.SheetData["creationComplete"] = true;

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

        #region Zobrazovaní postavy

        private static bool GetCharactersVisibleToAll(Campaign campaign)
        {
            if (campaign.GameSystemSettings.TryGetValue("charactersVisibleToAll", out var v))
            {
                if (v is bool b)
                    return b;

                if (v is System.Text.Json.JsonElement el)
                    return el.ValueKind == System.Text.Json.JsonValueKind.True;
            }

            return false;
        }

        #endregion

        #region Pomocné metody

        private static int? ToInt(object? value)
        {
            if (value is null)
                return null;

            if (value is int i)
                return i;

            if (value is long l)
                return (int)l;

            if (value is JsonElement el && el.ValueKind == JsonValueKind.Number)
                return el.GetInt32();

            if (int.TryParse(value.ToString(), out var parsed))
                return parsed;

            return null;
        }

        private static bool ToBool(object? value)
        {
            if (value is bool b)
                return b;

            if (value is System.Text.Json.JsonElement el)
                return el.ValueKind == System.Text.Json.JsonValueKind.True;

            return false;
        }
        #endregion
    }
}
