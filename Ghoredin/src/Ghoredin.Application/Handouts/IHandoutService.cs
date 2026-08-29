using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Handouts
{
    public interface IHandoutService
    {
        Task<HandoutDto> CreateAsync(CreateHandoutCommand command);
        Task<HandoutDto> UpdateAsync(UpdateHandoutCommand command);
        Task ShareAsync(Guid handoutId);
        Task UnshareAsync(Guid handoutId);
        Task DeleteAsync(Guid handoutId);
        Task<List<HandoutDto>> GetVisibleForCampaignAsync(Guid campaignId);
    }
}
