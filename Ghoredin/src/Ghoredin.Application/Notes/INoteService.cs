using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Notes
{
    public interface INoteService
    {
        Task<NoteDto> CreateAsync(CreateNoteCommand command);
        Task<NoteDto> UpdateAsync(UpdateNoteCommand command);
        Task<List<NoteDto>> GetCampaignNotesAsync(Guid campaignId);
        Task RevealSceneAsync(RevealSceneCommand command);
    }
}
