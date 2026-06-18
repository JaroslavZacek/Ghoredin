using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Domain.Campaigns
{
    public class Campaign
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string GameSystemId { get; set; } = string.Empty;

        public int? MaxPlayers { get; set; }

        public List<CampaignMember> Members { get; set; } = new();

        public Dictionary<string, object> GameSystemSettings { get; set; } = new();
    }
}
