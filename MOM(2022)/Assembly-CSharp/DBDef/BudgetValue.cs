namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("BUDGET_VALUE", "")]
    public class BudgetValue : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Value", true)]
        public int value;

        public static explicit operator BudgetValue(Enum e)
        {
            return DataBase.Get<BudgetValue>(e, false);
        }

        public static explicit operator BudgetValue(string e)
        {
            return DataBase.Get<BudgetValue>(e, true);
        }
    }
}

