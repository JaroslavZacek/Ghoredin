using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Domain.Notes
{
    public class CampaignNote
    {
        public Guid Id { get; set; }
        public Guid CampaignId { get; set; }
        public string AuthorUserId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string Content {  get; set; } = string.Empty;

        public string? PlayerFacingContent { get; set; }

        public NoteVisibility Visibility { get; set; } = NoteVisibility.GmOnly;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
