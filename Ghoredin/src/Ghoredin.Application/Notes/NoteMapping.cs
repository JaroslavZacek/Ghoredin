using Ghoredin.Domain.Notes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Notes
{
    public static class NoteMapping
    {
        public static NoteDto ToDto(this CampaignNote note, bool isGameMaster)
        {
            var content = isGameMaster ? note.Content : string.Empty;

            return new NoteDto(
                note.Id,
                note.CampaignId,
                note.AuthorUserId,
                note.Title,
                content,
                note.PlayerFacingContent,
                note.Visibility.ToString(),
                note.CreatedAt,
                note.UpdatedAt
            );
        }
    }
}
