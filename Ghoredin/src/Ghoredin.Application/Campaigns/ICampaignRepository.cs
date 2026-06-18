using Ghoredin.Domain.Campaigns;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Campaigns
{
    public interface ICampaignRepository
    {
        Task<Campaign?> GetByIdAsync(Guid id);
        Task<List<Campaign>> GetAllAsync();
        Task<List<Campaign>> GetByMemberAsync(string userId);
        Task AddAsync(Campaign campaign);
        Task SaveChangesAsync();
    }
}
