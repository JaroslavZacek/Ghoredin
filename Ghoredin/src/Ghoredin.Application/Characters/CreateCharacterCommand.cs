using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Characters
{
    public record CreateCharacterCommand(
        string Name,
        string GameSystemId,
        Dictionary<string, object> SheetData

     );
}
