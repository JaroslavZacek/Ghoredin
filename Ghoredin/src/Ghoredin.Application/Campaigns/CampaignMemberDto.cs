using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Campaigns
{
    public record CampaignMemberDto
    (
        Guid Id,
        string UserId,
        string Role,
        Guid? CharacterId
    );
}
