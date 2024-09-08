using DBUtils;
using MHUtils.UI;
using TMPro;

namespace MOM
{
    public class CombatLogDetails : TooltipBase
    {
        public TextMeshProUGUI labelAttacker;

        public TextMeshProUGUI labelDefender;

        public TextMeshProUGUI labelCenter;

        public GridItemManager gridAttacks;

        public override void Populate(object o)
        {
            BattleHUD.CombatSummary combatSummary = o as BattleHUD.CombatSummary;
            BattleHUD.CombatLogDetail combatLogDetail = combatSummary.details[0];
            if (combatLogDetail.right == null)
            {
                this.labelCenter.text = combatLogDetail.left;
                WizardColors.GetColor((PlayerWizard.Color)combatLogDetail.initiative);
                this.labelAttacker.text = "";
                this.labelDefender.text = "";
            }
            else
            {
                this.labelAttacker.text = combatLogDetail.right;
                this.labelDefender.text = combatLogDetail.left;
                this.labelCenter.text = Localization.Get("UI_VS", true);
                Battle battle = Battle.GetBattle();
                this.labelAttacker.color = WizardColors.GetColor(battle.attacker.wizard);
                this.labelDefender.color = WizardColors.GetColor(battle.defender.wizard);
            }
            this.gridAttacks.SetListItems(combatSummary.details.GetRange(1, combatSummary.details.Count - 1));
        }
    }
}
