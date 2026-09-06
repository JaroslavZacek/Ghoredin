using Ghoredin.Domain.Journal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Journal
{
    public interface IJournalRepository
    {
        Task<JournalEntry?> GetByOwnerAndCampaignAsync(Guid campaignId, string ownerUserId);
        Task AddAsync(JournalEntry journalEntry);
        Task SaveChangesAsync();
    }
}
