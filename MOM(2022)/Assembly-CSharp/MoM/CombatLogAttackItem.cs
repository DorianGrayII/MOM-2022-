using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace MOM
{
    public class CombatLogAttackItem : ListItem
    {
        public TextMeshProUGUI labelInitiative;

        public TextMeshProUGUI labelAttackerAction;

        public TextMeshProUGUI labelDefenderAction;

        public GameObject arrowLeft;

        public GameObject arrowRight;

        public Color sourceColour;

        public override void Set(object o, object data, int index)
        {
            BattleHUD.CombatLogDetail combatLogDetail = o as BattleHUD.CombatLogDetail;
            this.labelInitiative.text = ((combatLogDetail.initiative > 0) ? CombatLogAttackItem.ToRomanNumeral(combatLogDetail.initiative) : "");
            this.labelAttackerAction.text = combatLogDetail.left;
            this.labelDefenderAction.text = combatLogDetail.right;
            this.labelAttackerAction.color = ((combatLogDetail.arrow == BattleHUD.CombatLogDetail.Arrow.Right) ? this.sourceColour : Color.black);
            this.labelDefenderAction.color = ((combatLogDetail.arrow == BattleHUD.CombatLogDetail.Arrow.Left) ? this.sourceColour : Color.black);
            this.arrowLeft.SetActive(combatLogDetail.arrow == BattleHUD.CombatLogDetail.Arrow.Left);
            this.arrowRight.SetActive(combatLogDetail.arrow == BattleHUD.CombatLogDetail.Arrow.Right);
        }

        public static string ToRomanNumeral(int value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int num = value;
            while (num > 0)
            {
                if (num >= 1000)
                {
                    stringBuilder.Append("M");
                    num -= 1000;
                    continue;
                }
                if (num >= 900)
                {
                    stringBuilder.Append("CM");
                    num -= 900;
                    continue;
                }
                if (num >= 500)
                {
                    stringBuilder.Append("D");
                    num -= 500;
                    continue;
                }
                if (num >= 400)
                {
                    stringBuilder.Append("CD");
                    num -= 400;
                    continue;
                }
                if (num >= 100)
                {
                    stringBuilder.Append("C");
                    num -= 100;
                    continue;
                }
                if (num >= 90)
                {
                    stringBuilder.Append("XC");
                    num -= 90;
                    continue;
                }
                if (num >= 50)
                {
                    stringBuilder.Append("L");
                    num -= 50;
                    continue;
                }
                if (num >= 40)
                {
                    stringBuilder.Append("XL");
                    num -= 40;
                    continue;
                }
                if (num >= 10)
                {
                    stringBuilder.Append("X");
                    num -= 10;
                    continue;
                }
                if (num >= 9)
                {
                    stringBuilder.Append("IX");
                    num -= 9;
                    continue;
                }
                if (num >= 5)
                {
                    stringBuilder.Append("V");
                    num -= 5;
                    continue;
                }
                if (num >= 4)
                {
                    stringBuilder.Append("IV");
                    num -= 4;
                    continue;
                }
                if (num >= 1)
                {
                    stringBuilder.Append("I");
                    num--;
                    continue;
                }
                throw new Exception("Unexpected error.");
            }
            return stringBuilder.ToString();
        }
    }
}
