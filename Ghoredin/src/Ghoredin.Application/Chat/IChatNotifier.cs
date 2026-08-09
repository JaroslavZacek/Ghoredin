using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Chat
{
    public interface IChatNotifier
    {
        Task NotifyMessageAsync(ChatMessageDto message, string authorUserId, string? whisperToUserId);
    }
}
