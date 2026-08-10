using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Notes
{
    public record MoveNoteCommand(Guid NoteId, Guid? NewParentNoteId);
}
