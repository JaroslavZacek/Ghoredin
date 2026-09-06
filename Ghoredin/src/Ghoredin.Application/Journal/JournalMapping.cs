using System;
using System.Collections.Generic;
using System.Text;

using Ghoredin.Domain.Journal;

namespace Ghoredin.Application.Journal
{
    public static class JournalMapping
    {
        public static JournalEntryDto ToDto(this JournalEntry entry)
        {
            return new JournalEntryDto(
                entry.Id,
                entry.CampaignId,
                entry.Content,
                entry.UpdatedAt);
        }
    }
}
