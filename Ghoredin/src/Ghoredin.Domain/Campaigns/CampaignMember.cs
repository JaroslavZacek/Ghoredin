using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Domain.Campaigns
{
    public class CampaignMember
    {
        public Guid Id { get; set; }
        public Guid CampaignId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public CampaignRole Role { get; set; }
        public Guid? CharacterId { get; set; }
    }
}
