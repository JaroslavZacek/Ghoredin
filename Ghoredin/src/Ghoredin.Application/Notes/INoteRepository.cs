using Ghoredin.Domain.Notes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Notes
{
    public interface INoteRepository
    {
        Task<CampaignNote?> GetByIdAsync(Guid id);
        Task<List<CampaignNote>> GetByCampaignAsync(Guid campaignId);
        Task AddAsync(CampaignNote note);
        Task SaveChangesAsync();
    }
}
