namespace MOM
{
    using System;
    using System.Text;
    using TMPro;
    using UnityEngine;

    public class CombatLogAttackItem : ListItem
    {
        public TextMeshProUGUI labelInitiative;
        public TextMeshProUGUI labelAttackerAction;
        public TextMeshProUGUI labelDefenderAction;
        public GameObject arrowLeft;
        public GameObject arrowRight;
        public UnityEngine.Color sourceColour;

        public override void Set(object o, object data, int index)
        {
            BattleHUD.CombatLogDetail detail = o as BattleHUD.CombatLogDetail;
            this.labelInitiative.text = (detail.initiative > 0) ? ToRomanNumeral(detail.initiative) : "";
            this.labelAttackerAction.text = detail.left;
            this.labelDefenderAction.text = detail.right;
            this.labelAttackerAction.color = (detail.arrow == BattleHUD.CombatLogDetail.Arrow.Right) ? this.sourceColour : UnityEngine.Color.black;
            this.labelDefenderAction.color = (detail.arrow == BattleHUD.CombatLogDetail.Arrow.Left) ? this.sourceColour : UnityEngine.Color.black;
            this.arrowLeft.SetActive(detail.arrow == BattleHUD.CombatLogDetail.Arrow.Left);
            this.arrowRight.SetActive(detail.arrow == BattleHUD.CombatLogDetail.Arrow.Right);
        }

        public static string ToRomanNumeral(int value)
        {
            StringBuilder builder = new StringBuilder();
            int num = value;
            while (num > 0)
            {
                if (num >= 0x3e8)
                {
                    builder.Append("M");
                    num -= 0x3e8;
                    continue;
                }
                if (num >= 900)
                {
                    builder.Append("CM");
                    num -= 900;
                    continue;
                }
                if (num >= 500)
                {
                    builder.Append("D");
                    num -= 500;
                    continue;
                }
                if (num >= 400)
                {
                    builder.Append("CD");
                    num -= 400;
                    continue;
                }
                if (num >= 100)
                {
                    builder.Append("C");
                    num -= 100;
                    continue;
                }
                if (num >= 90)
                {
                    builder.Append("XC");
                    num -= 90;
                    continue;
                }
                if (num >= 50)
                {
                    builder.Append("L");
                    num -= 50;
                    continue;
                }
                if (num >= 40)
                {
                    builder.Append("XL");
                    num -= 40;
                    continue;
                }
                if (num >= 10)
                {
                    builder.Append("X");
                    num -= 10;
                    continue;
                }
                if (num >= 9)
                {
                    builder.Append("IX");
                    num -= 9;
                    continue;
                }
                if (num >= 5)
                {
                    builder.Append("V");
                    num -= 5;
                    continue;
                }
                if (num >= 4)
                {
                    builder.Append("IV");
                    num -= 4;
                    continue;
                }
                if (num < 1)
                {
                    throw new Exception("Unexpected error.");
                }
                builder.Append("I");
                num--;
            }
            return builder.ToString();
        }
    }
}

