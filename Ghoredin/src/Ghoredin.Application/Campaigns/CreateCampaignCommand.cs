using System;
using System.Collections.Generic;
using System.Text;

using Ghoredin.Application.GameSystems;

namespace Ghoredin.Application.Campaigns
{
    public record CreateCampaignCommand
    (
        string Name,
        string GameSystemId,
        int? MaxPlayers,
        CharacterCreationMethod CharacterCreationMethod
    );
}
