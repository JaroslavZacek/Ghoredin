using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Domain.Notes
{
    public class Handout
    {
        public Guid Id { get; set; }
        public Guid CamapaignId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content {  get; set; } = string.Empty;
        public HandoutShareMode ShareMode { get; set; }
        public bool IsShared { get; set; }
        public string? SnapshotContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
