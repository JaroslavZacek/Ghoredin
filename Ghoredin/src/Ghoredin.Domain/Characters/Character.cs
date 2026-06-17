using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Domain.Characters
{
    public class Character
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string GameSystem { get; set; } = string.Empty;

        public string OwnerUserId { get; set; } = string.Empty;
        public Guid? CampaignId { get; set; }

        public string? PortraitUrl { get; set; }

        public Dictionary<string, object> SheetData { get; set; } = new();

    }
}
