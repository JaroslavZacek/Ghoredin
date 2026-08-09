using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Chat
{
    public record ChatMessageDto(
        Guid Id,
        Guid CampaignId,
        string AuthorUserId,
        string Content,
        bool IsWhisper,
        DateTime CreatedAt
    );
}
