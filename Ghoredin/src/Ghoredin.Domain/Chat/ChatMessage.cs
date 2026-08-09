using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Domain.Chat
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public Guid CampaignId { get; set; }
        public string AuthorUserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public string? WhisperToUserId { get; set; }

        public DateTime CreateAt { get; set; }
    }
}
