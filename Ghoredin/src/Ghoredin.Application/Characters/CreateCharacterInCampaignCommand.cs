using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Characters
{
    public record CreateCharacterInCampaignCommand
    (
        Guid CampaignId,
        string Name,
        Dictionary<string, object> SheetData
    );

}
