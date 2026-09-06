using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Domain.Journal
{
    public class JournalEntry
    {
        public Guid Id { get; set; }
        public Guid CampaignId { get; set; }
        public string OwnerUserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
