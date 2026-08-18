using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Notes
{
    public record ReorderSiblingsCommand(
        Guid CampaignId,
        Guid? ParentNoteId,
        List<Guid> OrderedNoteIds);
}
