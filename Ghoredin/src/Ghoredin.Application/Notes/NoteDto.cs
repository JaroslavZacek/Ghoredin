using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Notes
{
    public record NoteDto
    (
        Guid Id,
        Guid CampaignId,
        string AuthorUserId,
        string Title,
        string Content,
        string? PlayerFacingContent,
        string Visibility,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
