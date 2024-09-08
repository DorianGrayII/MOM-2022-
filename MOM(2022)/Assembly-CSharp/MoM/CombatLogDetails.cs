namespace MOM
{
    using DBUtils;
    using MHUtils.UI;
    using System;
    using TMPro;

    public class CombatLogDetails : TooltipBase
    {
        public TextMeshProUGUI labelAttacker;
        public TextMeshProUGUI labelDefender;
        public TextMeshProUGUI labelCenter;
        public GridItemManager gridAttacks;

        public override void Populate(object o)
        {
            BattleHUD.CombatSummary summary = o as BattleHUD.CombatSummary;
            BattleHUD.CombatLogDetail detail = summary.details[0];
            if (detail.right == null)
            {
                this.labelCenter.text = detail.left;
                WizardColors.GetColor((PlayerWizard.Color) detail.initiative);
                this.labelAttacker.text = "";
                this.labelDefender.text = "";
            }
            else
            {
                this.labelAttacker.text = detail.right;
                this.labelDefender.text = detail.left;
                this.labelCenter.text = Localization.Get("UI_VS", true, Array.Empty<object>());
                Battle battle = Battle.GetBattle();
                this.labelAttacker.color = WizardColors.GetColor(battle.attacker.wizard);
                this.labelDefender.color = WizardColors.GetColor(battle.defender.wizard);
            }
            this.gridAttacks.SetListItems<BattleHUD.CombatLogDetail>(summary.details.GetRange(1, summary.details.Count - 1), null, null);
        }
    }
}

