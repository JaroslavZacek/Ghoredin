using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Campaigns
{
    public record JoinCampaignCommand
    (
        Guid CampaignId,
        Guid? CharacterId
    );
}
