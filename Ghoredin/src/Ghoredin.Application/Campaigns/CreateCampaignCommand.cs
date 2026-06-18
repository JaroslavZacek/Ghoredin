using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Campaigns
{
    public record CreateCampaignCommand
    (
        string Name,
        string GameSystemId,
        int? MaxPlayers
    );
}
