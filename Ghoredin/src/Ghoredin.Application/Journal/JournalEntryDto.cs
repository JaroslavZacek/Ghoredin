using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Journal
{
    public record JournalEntryDto(
        Guid Id,
        Guid CampaignId,
        string Content,
        DateTime UpdatedAt);
}
