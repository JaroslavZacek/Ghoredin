using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Characters
{
    public record CharacterDto(
        Guid Id,
        string Name,
        string GameSystemId,
        Guid? CampaignId,
        string? PortraitUrl,
        Dictionary<string, object> SheetData
    );
}
