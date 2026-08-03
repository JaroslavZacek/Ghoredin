using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using Ghoredin.Application.Dice;
using Ghoredin.Application.GameSystems;

namespace Ghoredin.Infrastructure.GameSystems
{
    public class Dnd5eGameSystem : IGameSystem
    {       
        public string Id => "dnd5e";
        public string DisplayName => "D&D 5e";
        public bool SupportsLeveling => true;

        private readonly IDiceService _dice;

        private static readonly string[] AbilityNames =
        {   
            "Strength",
            "Dexterity",
            "Constitution",
            "Intelligence",
            "Wisdom",
            "Charisma"
        };

        public Dnd5eGameSystem(IDiceService dice)
        {
            _dice = dice;
        }

        public Dictionary<string, object> CreateDefaultSheet()
        {
            var abilities = new Dictionary<string, object>();

            foreach(var name in AbilityNames)
                abilities[name] = 10;

            return new Dictionary<string, object>
            {
                ["abilities"] = abilities,
                ["hitPoints"] = new Dictionary<string, object>
                {
                    ["current"] = 10,
                    ["max"] = 10
                }
            };
        }

        #region Validace

        public ValidationResult ValidateSheet(Dictionary<string, object> sheetData)
        {
            var errors = new List<string>();


            // Ověření existence sekce abilities a jednotlivých atributů
            if (!sheetData.TryGetValue("abilities", out var abilitiesObj))
            {
                errors.Add("Chybí sekce ´abilities´.");
            }
            else
            {
                var abilities = ToDictionary(abilitiesObj);

                foreach (var name in AbilityNames)
                {
                    if (!abilities.TryGetValue(name, out var val))
                    {
                        errors.Add($"Chybí atribut '{name}'.");
                        continue;
                    }

                    var num = ToInt(val);

                    if (num is null)
                        errors.Add($"Atribut '{name}' není číslo.");
                    else if (num < 1 || num > 30)
                        errors.Add($"Atribut '{name}' musí být 1-30 (nyní je {num}).");
                }
            }

            // Ověření existence sekce hitPoints a hodnot max a current
            if (!sheetData.TryGetValue("hitPoints", out var hpObj))
                errors.Add("Chybí sekce 'hitPoints'.");

            else
            {
                var hp = ToDictionary(hpObj);
                var maxHp = ToInt(hp.GetValueOrDefault("max"));
                var currentHp = ToInt(hp.GetValueOrDefault("current"));

                if (maxHp is null || maxHp < 1)
                    errors.Add("Max HP musí být kladné číslo.");

                if (currentHp is null)
                    errors.Add("Aktuální HP není číslo.");
            }

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }

        // Validace point buy systému tvorby postavy
        public ValidationResult ValidatePointBuy(Dictionary<string, object> abilities)
        {
            var errors = new List<string>();

            int totalCost = 0;

            foreach (var name in AbilityNames)
            {
                var val = ToInt(abilities.GetValueOrDefault(name));

                if (val is null)
                {
                    errors.Add($"Chybí atribut '{name}'.");
                }

                if (!PointBuyCost.ContainsKey(val.Value))
                {
                    errors.Add($"Atribut '{name}' musí být 8-15 (je {val}.");
                    continue;
                }

                totalCost += PointBuyCost[val.Value];
            }

            if (totalCost > PointBuyBudget)
                errors.Add($"Překročen rozpočet bodů ({totalCost}/{PointBuyBudget}).");

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }

        // Validace standardního pole hodnot atributů
        public ValidationResult ValidateStandardArray(Dictionary<string, object> abilities)
        {
            var errors = new List<string>();
            var used = new List<int>();

            foreach (var name in AbilityNames)
            {
                var val = ToInt(abilities.GetValueOrDefault(name));

                if (val is null)
                {
                    errors.Add($"Chybí atribut '{name}'.");
                    continue;
                }
                used.Add(val.Value);
            }

            // Porovnání seznamu použitých hodnot s očekávaným standardním polem
            if (errors.Count == 0)
            {
                var sortedUsed = used.OrderBy(x => x).ToArray();
                var sortedArray = StandardArray.OrderBy(x => x).ToArray();

                if (!sortedUsed.SequenceEqual(sortedArray))
                    errors.Add("Musíš použít přesně sadu 15, 14, 13, 12, 10, 8 (každé jednou).");
            }

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }

        #endregion

        #region Tvorba postavy

        // Point buy: kolik bodů stojí která hodnota atributu
        private static readonly Dictionary<int, int> PointBuyCost = new()
        {
            [8] = 0,
            [9] = 1,
            [10] = 2,
            [11] = 3,
            [12] = 4,
            [13] = 5,
            [14] = 7,
            [15] = 9
        };

        private const int PointBuyBudget = 27;

        // Standard array: pevná sada hodnot atributů
        private static readonly int[] StandardArray = { 15, 14, 13, 12, 10, 8 };

        #endregion

        #region pomocné metody pro práci s JSON hodnotami

        private static Dictionary<string, object> ToDictionary(object? value)
        {
            if (value is Dictionary<string, object> dict)
                return dict;
            if (value is JsonElement el && el.ValueKind == JsonValueKind.Object)
            {
                var result = new Dictionary<string, object>();

                foreach (var prop in el.EnumerateObject())
                    result[prop.Name] = prop.Value;

                return result;
            }

            return new Dictionary<string, object>();
        }

        private static int? ToInt(object? value)
        {
            if (value is null)
                return null;

            if (value is int i)
                return i;

            if (value is long l)
                return (int)l;

            if (value is JsonElement el && el.ValueKind == JsonValueKind.Number)
                return el.GetInt32();

            if (int.TryParse(value.ToString(), out var parsed))
                return parsed;

            return null;
        }

        #endregion

        #region Pomocné metody

        private int Roll4d6DropLowest()
        {
            var rolls = _dice.RollMany(DiceType.D6, 4);

            return rolls.OrderByDescending(r => r).Take(3).Sum();
        }

        #endregion

    }
}
