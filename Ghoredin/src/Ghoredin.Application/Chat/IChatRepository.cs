using System;
using System.Collections.Generic;
using System.Text;

using Ghoredin.Domain.Chat;

namespace Ghoredin.Application.Chat
{
    public interface IChatRepository
    {
        Task<List<ChatMessage>> GetByCampaignAsync(Guid campaignId, int take = 50);
        Task AddAsync(ChatMessage message);
        Task SaveChangesAsync();
    }
}
