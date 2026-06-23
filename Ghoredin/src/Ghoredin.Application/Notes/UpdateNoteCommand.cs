using Ghoredin.Domain.Notes;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Notes
{
    public record UpdateNoteCommand
    (
        Guid Id,
        string Title,
        string Content,
        string? PlayerFacingContent,
        NoteVisibility Visibility
    );
}
