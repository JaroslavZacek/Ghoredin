using Ghoredin.Domain.Characters;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Characters
{
    public interface ICharacterService
    {
        Task<CharacterDto> CreateAsync(CreateCharacterCommand command);
        Task<List<CharacterDto>> GetMyCharactersAsync();
        Task<CharacterDto> GetByIdAsync(Guid id);
        Task<CharacterDto> CreateInCampaignAsync(CreateCharacterInCampaignCommand command);
        Task<List<CharacterDto>> GetCampaignCharactersAsync(Guid campaignId);
    }
}
