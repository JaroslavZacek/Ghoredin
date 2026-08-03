using Ghoredin.Application.Dice;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Infrastructure.Dice
{
    public class DiceService : IDiceService
    {
        private readonly Random _random = new();

        /// <summary>
        /// Hození kostkou s daným počtem stran (dice)
        /// </summary>
        /// <param name="dice">Počet stran kostky</param>
        /// <returns>Náhodné číslo mezi 1 a počtem stran kostky</returns>
        public int Roll(DiceType dice)
        {
            int sides = (int)dice;

            return _random.Next(1, sides + 1);
        }

        /// <summary>
        /// Hození kostkou s daným počtem stran (dice) vícekrát (count)
        /// </summary>
        /// <param name="dice">Počet stran kostky</param>
        /// <param name="count">Počet kostek, které se mají hodit</param>
        /// <returns>Pole náhodných čísel</returns>
        public int[] RollMany(DiceType dice, int count)
        {
            var results = new int[count];

            for (int i = 0; i < count; i++)
                results[i] = Roll(dice);

            return results;
        }
    }
}
