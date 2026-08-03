using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.GameSystems
{
    public interface IGameSystem
    {
        string Id { get; }

        string DisplayName { get; }

        bool SupportsLeveling { get; }

        Dictionary<string, object> CreateDefaultSheet();

        ValidationResult ValidateSheet(Dictionary<string, object> sheetData);

        ValidationResult ValidateAbilityScores(Dictionary<string, object> abilities, string creationMethod);

        int? RollSingleAbilityScore();
    }
}
