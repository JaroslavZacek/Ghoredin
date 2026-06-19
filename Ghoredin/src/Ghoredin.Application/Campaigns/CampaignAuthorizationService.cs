using Ghoredin.Domain.Campaigns;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Campaigns
{
    public class CampaignAuthorizationService : ICampaignAuthorizationService
    {
        public bool IsGameMaster(Campaign campaign, string userId)
        {
            return campaign.Members.Any(m => m.UserId == userId && m.Role == CampaignRole.GameMaster);
        }

        public bool IsMemeber(Campaign campaign, string userId)
        {
            return campaign.Members.Any(m => m.UserId == userId);
        }
    }
}
