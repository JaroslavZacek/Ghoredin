using Ghoredin.Application.Campaigns;
using Ghoredin.Domain.Campaigns;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Infrastructure.Persistence.Repositories
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly AppDbContext _context;

        public CampaignRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Campaign> GetByIdAsync(Guid id)
        {
            return await _context.Campaigns
                            .Include(c => c.Members)
                            .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Campaign>> GetAllAsync()
        {
            return await _context.Campaigns
                            .Include(c => c.Members)
                            .ToListAsync();
        }

        public async Task<List<Campaign>> GetByMemberAsync(string userId)
        {
            return await _context.Campaigns
                            .Include(c => c.Members)
                            .Where(c => c.Members.Any(m => m.UserId == userId))
                            .ToListAsync();
        }

        public async Task AddAsync(Campaign campaign)
        {
            await _context.Campaigns.AddAsync(campaign);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
