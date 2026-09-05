using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Handouts
{
    public record HandoutDto(
        Guid Id,
        Guid CampaignId,
        string Title,
        string ContentType,
        string Content,
        string ShareMode,
        bool IsShared,
        DateTime UpdatedAt);
}
