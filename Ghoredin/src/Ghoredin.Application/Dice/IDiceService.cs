using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Dice
{
    public interface IDiceService
    {
        int Roll(DiceType dice);

        int[] RollMany(DiceType dice, int count);
    }
}
