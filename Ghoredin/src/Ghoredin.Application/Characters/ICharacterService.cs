using Ghoredin.Domain.Characters;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Characters
{
    public interface ICharacterService
    {
        Task<List<CharacterDto>> GetMyCharactersAsync();
        Task<CharacterDto> GetByIdAsync(Guid id);
        Task<CharacterDto> CreateInCampaignAsync(CreateCharacterInCampaignCommand command);
        Task<List<CharacterDto>> GetCampaignCharactersAsync(Guid campaignId);
        Task<CharacterDto> StartRolledCharacterAsync(Guid campaignId, string name);
        Task<CharacterDto> RollAbilityAsync(Guid characterId, string abilityName);
        Task<CharacterDto> CompleteRolledCharacterAsync(Guid characterId);
    }
}
