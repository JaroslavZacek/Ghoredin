using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Journal
{
    public record SaveJournalCommand(
        Guid CampaignId,
        string Content);
}
