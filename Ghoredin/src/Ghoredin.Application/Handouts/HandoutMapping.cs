using Ghoredin.Domain.Handouts;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Handouts
{
    public static class HandoutMapping
    {
        public static HandoutDto ToDto(this Handout handout, bool isGameMaster)
        {
            var content = isGameMaster
                ? handout.Content
                : (handout.ShareMode == HandoutShareMode.Snapshot
                    ? handout.SnapshotContent ?? string.Empty
                    : handout.Content);

            return new HandoutDto(
                handout.Id,
                handout.CampaignId,
                handout.Title,
                handout.ContentType.ToString(),
                content,
                handout.ShareMode.ToString(),
                handout.IsShared,
                handout.UpdatedAt);
        }
    }
}
