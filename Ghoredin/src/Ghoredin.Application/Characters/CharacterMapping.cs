using Ghoredin.Domain.Characters;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Characters
{
    public static class CharacterMapping
    {
        public static CharacterDto ToDto(this Character character)
        {
            return new CharacterDto(
                character.Id,
                character.Name,
                character.GameSystemId,
                character.CampaignId,
                character.PortraitUrl,
                character.SheetData
            );
        }
    }
}
