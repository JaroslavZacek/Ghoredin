using Ghoredin.Application.Handouts;
using Ghoredin.Domain.Handouts;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Infrastructure.Persistence.Repositories
{
    public class HandoutRepository: IHandoutRepository
    {
        private readonly AppDbContext _context;

        public HandoutRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Handout?> GetByIdAsync(Guid id)
        {
            return await _context.Handouts.FindAsync(id);
        }

        public async Task<List<Handout>> GetByCampaignAsync(Guid campaignId)
        {
            return await _context.Handouts
                .Where(h => h.CampaignId == campaignId)
                .OrderByDescending(h => h.UpdatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Handout handout)
        {
            await _context.Handouts.AddAsync(handout);
        }

        public async Task DeleteAsync(Handout handout)
        {
            _context.Handouts.Remove(handout);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
