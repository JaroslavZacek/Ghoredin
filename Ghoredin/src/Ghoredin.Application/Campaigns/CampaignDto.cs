using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Campaigns
{
    public record CampaignDto
    (
        Guid Id,
        string Name,
        string GameSystemId,
        int? MaxPlayers,
        int PlayerCount,
        List<CampaignMemberDto> Members
    );
}
