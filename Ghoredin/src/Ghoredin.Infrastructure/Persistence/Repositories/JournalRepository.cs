using Ghoredin.Application.Journal;
using Ghoredin.Domain.Journal;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Infrastructure.Persistence.Repositories
{
    public class JournalRepository : IJournalRepository
    {
        private readonly AppDbContext _context;

        public JournalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<JournalEntry> GetByOwnerAndCampaignAsync(Guid campaignId, string ownerUserId)
        {
            return await _context.JournalEntries
                .FirstOrDefaultAsync(j => j.CampaignId == campaignId && j.OwnerUserId == ownerUserId);
        }
        
        public async Task AddAsync(JournalEntry entry)
        {
            await _context.JournalEntries.AddAsync(entry);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
