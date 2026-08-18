using System;
using System.Collections.Generic;
using System.Text;

using Ghoredin.Domain.Notes

namespace Ghoredin.Application.Handouts
{
    public interface IHandoutRepository
    {
        Task<Handout?> GetByIdAsync(Guid id);
        Task<List<Handout>> GetByCampaignAsync(Guid campaignId);
        Task AddAsync(Handout handout);
        Task DeleteAsync(Handout handout);
        Task SaveChangesAsync();
    }
}
