using Ghoredin.Domain.Notes;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Handouts
{
    public record CreateHandoutCommand(
        Guid CampaignId,
        string Title,
        string Content,
        HandoutShareMode ShareMode);
}
