using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Chat
{
    public interface IChatService
    {
        Task<ChatMessageDto> SendMessageAsync(SendMessageCommand command);
        Task<List<ChatMessageDto>> GetVisibleHistoryAsync(Guid campaignId);
    }
}
