using Ghoredin.Application.Notes;
using Ghoredin.Domain.Notes;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Infrastructure.Persistence.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly AppDbContext _context;

        public NoteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CampaignNote?> GetByIdAsync(Guid id)
        {
            return await _context.CampaignNotes.FindAsync(id);
        }

        public async Task<List<CampaignNote>> GetByCampaignAsync(Guid campaignId)
        {
            return await _context.CampaignNotes
                .Where(n => n.CampaignId == campaignId)
                .OrderByDescending(n => n.UpdatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(CampaignNote note)
        {
            await _context.CampaignNotes.AddAsync(note);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
