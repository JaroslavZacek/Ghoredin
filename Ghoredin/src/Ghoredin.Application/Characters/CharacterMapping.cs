using Ghoredin.Domain.Characters;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Characters
{
    public static class CharacterMapping
    {
        public static CharacterDto ToDto(this Character character, string? campaignName = null, bool campaignActive = true)
        {
            return new CharacterDto(
                character.Id,
                character.Name,
                character.GameSystemId,
                character.CampaignId,
                campaignName,
                campaignActive,
                character.PortraitUrl,
                character.SheetData
            );
        }
    }
}
