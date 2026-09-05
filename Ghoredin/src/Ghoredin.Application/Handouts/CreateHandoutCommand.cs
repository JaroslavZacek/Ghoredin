using Ghoredin.Domain.Handouts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Handouts
{
    public record CreateHandoutCommand(
        Guid CampaignId,
        string Title,
        string Content,
        HandoutContentType ContentType,
        HandoutShareMode ShareMode);
}
