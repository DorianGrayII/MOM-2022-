using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
    [ClassPrototype("BUDGET_SCALING", "")]
    public class BudgetScaling : DBClass
    {
        public static string abbreviation = "";

        [Prototype("TurnToBudget", true)]
        public TurnToBudget[] turnToBudget;

        public static explicit operator BudgetScaling(Enum e)
        {
            return DataBase.Get<BudgetScaling>(e);
        }

        public static explicit operator BudgetScaling(string e)
        {
            return DataBase.Get<BudgetScaling>(e, reportMissing: true);
        }

        public void Set_turnToBudget(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.turnToBudget = new TurnToBudget[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is TurnToBudget))
                {
                    Debug.LogError("turnToBudget of type TurnToBudget received invalid type from array! " + list[i]);
                }
                this.turnToBudget[i] = list[i] as TurnToBudget;
            }
        }
    }
}
