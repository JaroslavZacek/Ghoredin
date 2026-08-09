using System;
using System.Collections.Generic;
using System.Text;

using Ghoredin.Domain.Chat;

namespace Ghoredin.Application.Chat
{
    public static class ChatMapping
    {
        public static ChatMessageDto ToDto(this ChatMessage message)
        {
            return new ChatMessageDto(
                message.Id,
                message.CampaignId,
                message.AuthorUserId,
                message.Content,
                message.WhisperToUserId is not null,
                message.CreateAt
                );
        }
    }
}
