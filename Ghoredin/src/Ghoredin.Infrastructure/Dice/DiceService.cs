using Ghoredin.Application.Dice;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Infrastructure.Dice
{
    public class DiceService : IDiceService
    {
        private readonly Random _random = new();

        public int Roll(DiceType dice)
        {
            int sides = (int)dice;

            return _random.Next(1, sides + 1);
        }

        public int[] RollMany(DiceType dice, int count)
        {
            var results = new int[count];

            for (int i = 0; i < count; i++)
                results[i] = Roll(dice);

            return results;
        }
    }
}
