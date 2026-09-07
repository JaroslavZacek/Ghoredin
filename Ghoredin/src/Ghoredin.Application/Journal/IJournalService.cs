using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Journal
{
    public interface IJournalService
    {
        Task<JournalEntryDto> SaveAsync(SaveJournalCommand command);
        Task<JournalEntryDto> GetMyEntryAsync(Guid campaignId);
    }
}
