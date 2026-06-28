using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Notes
{
    public record RevealSceneCommand
    (
        Guid CampaignId,
        Guid NoteId,
        List<string> TargetUserIds
    );
}
