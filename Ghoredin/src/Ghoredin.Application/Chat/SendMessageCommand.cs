using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Chat
{
    public record SendMessageCommand(
        Guid CampaignId,
        string Content,
        string? WhisperToUserId
    );
}
