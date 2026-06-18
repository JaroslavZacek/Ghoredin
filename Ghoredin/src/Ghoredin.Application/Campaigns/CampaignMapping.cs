using Ghoredin.Domain.Campaigns;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Campaigns
{
    public static class CampaignMapping
    {
        public static CampaignDto ToDto(this Campaign campaign)
        {
            return new CampaignDto(
                campaign.Id,
                campaign.Name,
                campaign.GameSystemId,
                campaign.MaxPlayers,
                campaign.Members.Count(m => m.Role == CampaignRole.Player),
                campaign.Members.Select(m => m.ToDto()).ToList()
            );
        }

        public static CampaignMemberDto ToDto(this CampaignMember member)
        {
            return new CampaignMemberDto(
                member.Id,
                member.UserId,
                member.Role.ToString(),
                member.CharacterId
            );
        }
    }
}
