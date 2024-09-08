namespace DBDef
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ClassPrototype("BUDGET_SCALING", "")]
    public class BudgetScaling : DBClass
    {
        public static string abbreviation = "";
        [Prototype("TurnToBudget", true)]
        public TurnToBudget[] turnToBudget;

        public static explicit operator BudgetScaling(Enum e)
        {
            return DataBase.Get<BudgetScaling>(e, false);
        }

        public static explicit operator BudgetScaling(string e)
        {
            return DataBase.Get<BudgetScaling>(e, true);
        }

        public void Set_turnToBudget(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.turnToBudget = new TurnToBudget[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is TurnToBudget))
                    {
                        string text1;
                        object local1 = list[i];
                        if (local1 != null)
                        {
                            text1 = local1.ToString();
                        }
                        else
                        {
                            object local2 = local1;
                            text1 = null;
                        }
                        Debug.LogError("turnToBudget of type TurnToBudget received invalid type from array! " + text1);
                    }
                    this.turnToBudget[i] = list[i] as TurnToBudget;
                }
            }
        }
    }
}

