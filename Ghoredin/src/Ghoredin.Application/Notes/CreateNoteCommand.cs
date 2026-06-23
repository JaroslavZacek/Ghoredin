using Ghoredin.Domain.Notes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Notes
{
    public record CreateNoteCommand
    (
        Guid CamapignId,
        string Title,
        string Content,
        string? PlayerFacingContent,
        NoteVisibility Visibility
    );
}
