using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Campaigns
{
    public interface ICampaignService
    {
        Task<CampaignDto> CreateAsync(CreateCampaignCommand command);
        Task<List<CampaignDto>> GetMyCampaignsAsync();
        Task<CampaignDto?> GetByIdAsync(Guid id); 
        Task JoinAsync(JoinCampaignCommand command);
        Task<List<CampaignDto>> GetAvailableCampaignsAsync();
    }
}
