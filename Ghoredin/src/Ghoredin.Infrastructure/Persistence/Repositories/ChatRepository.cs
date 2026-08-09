using System;
using System.Collections.Generic;
using System.Text;

using Ghoredin.Application.Chat;
using Ghoredin.Domain.Chat;

using Microsoft.EntityFrameworkCore;

namespace Ghoredin.Infrastructure.Persistence.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly AppDbContext _context;

        public ChatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatMessage>> GetByCampaignAsync(Guid campaignId, int take = 50)
        {
            var messages = await _context.ChatMessages
                .Where(m => m.CampaignId == campaignId)
                .OrderByDescending(m => m.CreatedAt)
                .Take(take)
                .ToListAsync();

            messages.Reverse();

            return messages;
        }

        public async Task AddAsync(ChatMessage message)
        {
            await _context.ChatMessages.AddAsync(message);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
