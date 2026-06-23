using Ghoredin.Domain.Notes;

namespace Ghoredin.Server.Requests
{
    public record UpdateNoteRequest(
        string Title,
        string Content,
        string? PlayerFacingContent,
        NoteVisibility Visibility
        );
}
