using System;
using MHUtils;

namespace DBDef
{
    [ClassPrototype("TURN_TO_BUDGET", "")]
    public class TurnToBudget : DBClass
    {
        public static string abbreviation = "";

        [Prototype("Turn", true)]
        public int turn;

        [Prototype("Budget", true)]
        public int budget;

        public static explicit operator TurnToBudget(Enum e)
        {
            return DataBase.Get<TurnToBudget>(e);
        }

        public static explicit operator TurnToBudget(string e)
        {
            return DataBase.Get<TurnToBudget>(e, reportMissing: true);
        }
    }
}
