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
            var creation = campaign.GameSystemSettings.TryGetValue("characterCreation", out var m)
                ? m?.ToString() ?? "PointBuy"
                : "PointBuy";

            var visibleToAll = campaign.GameSystemSettings.TryGetValue("charactersVisibleToAll", out var v)
                && v is bool b && b;

            return new CampaignDto(
                campaign.Id,
                campaign.Name,
                campaign.GameSystemId,
                campaign.MaxPlayers,
                campaign.Members.Count(m => m.Role == CampaignRole.Player && m.IsActive),
                creation,
                visibleToAll,
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
