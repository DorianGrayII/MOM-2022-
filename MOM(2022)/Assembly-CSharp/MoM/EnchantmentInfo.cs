using DBDef;
using DBUtils;
using TMPro;
using UnityEngine;

namespace MOM
{
    public class EnchantmentInfo : EnchantmentListItem
    {
        public TextMeshProUGUI labelDescription;

        public TextMeshProUGUI labelUpkeepCost;

        public TextMeshProUGUI labelController;

        public TextMeshProUGUI labelDuration;

        public TextMeshProUGUI labelCanBeDispelled;

        public TextMeshProUGUI labelTarget;

        public GameObjectEnabler<ERealm> realm;

        public GameObject descriptionVisibility;

        public GameObject upkeepVisibility;

        public GameObject controllerVisibility;

        public GameObject durationVisibility;

        public GameObject canBeDispelledVisibility;

        public GameObject targetVisibility;

        public override void Set(object e)
        {
            base.Set(e);
            Enchantment enchantment = e as Enchantment;
            EnchantmentInstance enchantmentInstance = e as EnchantmentInstance;
            Spell spell = null;
            if (enchantmentInstance != null)
            {
                enchantment = enchantmentInstance.source;
            }
            else
            {
                spell = enchantment.GetSpell();
            }
            if (enchantment == null)
            {
                return;
            }
            if ((bool)this.labelDescription)
            {
                this.labelDescription.text = enchantment.GetDescriptionInfo().GetLocalizedDescription();
            }
            if ((bool)this.durationVisibility)
            {
                this.durationVisibility.gameObject.SetActive(enchantmentInstance != null);
            }
            if ((bool)this.controllerVisibility)
            {
                this.controllerVisibility.gameObject.SetActive(enchantmentInstance != null);
            }
            if ((bool)this.targetVisibility)
            {
                this.targetVisibility.gameObject.SetActive(enchantmentInstance != null || spell != null);
            }
            if (enchantmentInstance != null)
            {
                PlayerWizard playerWizard = enchantmentInstance.owner?.GetEntity() as PlayerWizard;
                if ((bool)this.labelUpkeepCost)
                {
                    this.labelUpkeepCost.text = enchantmentInstance.GetEnchantmentCost(playerWizard).ToString();
                }
                if ((bool)this.labelDuration)
                {
                    this.labelDuration.text = enchantmentInstance.countDown.ToString();
                }
                if ((bool)this.durationVisibility)
                {
                    this.durationVisibility.gameObject.SetActive(enchantmentInstance.countDown > 0);
                }
                if ((bool)this.controllerVisibility)
                {
                    this.controllerVisibility.gameObject.SetActive(playerWizard != null);
                }
                if (playerWizard != null && (bool)this.labelController)
                {
                    string text = ((!playerWizard.IsHuman) ? (GameManager.GetHumanWizard().GetDiscoveredWizards().Contains(playerWizard) ? playerWizard.name : global::DBUtils.Localization.Get("UI_UNKNOWN_WIZARD", true)) : playerWizard.name);
                    this.labelController.text = text;
                }
                if ((bool)this.labelTarget)
                {
                    this.labelTarget.text = enchantmentInstance.manager.owner.GetName();
                }
            }
            else
            {
                if ((bool)this.labelUpkeepCost)
                {
                    this.labelUpkeepCost.text = enchantment.upkeepCost.ToString();
                }
                if (spell != null && (bool)this.labelTarget)
                {
                    this.labelTarget.text = spell.targetType.desType.GetLocalizedTargetTypeDescription();
                }
                if ((bool)this.durationVisibility)
                {
                    this.durationVisibility.gameObject.SetActive(enchantment.lifeTime > 0);
                }
                if ((bool)this.labelDuration)
                {
                    this.labelDuration.text = enchantment.lifeTime.ToString();
                }
            }
            if ((bool)this.labelCanBeDispelled)
            {
                this.labelCanBeDispelled.text = global::DBUtils.Localization.Get(enchantment.allowDispel ? "UI_YES" : "UI_NO", true);
            }
            if (this.realm != null)
            {
                this.realm.Set(enchantment.realm);
            }
        }
    }
}
