using Ghoredin.Domain.Campaigns;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Campaigns
{
    public interface ICampaignAuthorizationService
    {
        bool IsGameMaster(Campaign campaign, string userId);
        bool IsMember(Campaign campaign, string userId);
    }
}
