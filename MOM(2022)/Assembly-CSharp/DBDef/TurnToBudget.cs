namespace DBDef
{
    using MHUtils;
    using System;

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
            return DataBase.Get<TurnToBudget>(e, false);
        }

        public static explicit operator TurnToBudget(string e)
        {
            return DataBase.Get<TurnToBudget>(e, true);
        }
    }
}

