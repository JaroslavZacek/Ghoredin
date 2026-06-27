using Ghoredin.Domain.Notes;

namespace Ghoredin.Server.Requests
{
    public record CreateNoteRequest
    (
        Guid CampaignId,
        string Title,
        string Content,
        string? PlayerFacingContent,
        NoteVisibility Visibility
    );
}
