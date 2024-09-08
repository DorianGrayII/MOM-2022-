namespace MOM
{
    using DBDef;
    using DBUtils;
    using System;
    using TMPro;
    using UnityEngine;

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
            Enchantment source = e as Enchantment;
            EnchantmentInstance instance = e as EnchantmentInstance;
            Spell spell = null;
            if (instance != null)
            {
                source = (Enchantment) instance.source;
            }
            else
            {
                spell = EnchantmentExtension.GetSpell(source);
            }
            if (source != null)
            {
                if (this.labelDescription)
                {
                    this.labelDescription.text = source.GetDescriptionInfo().GetLocalizedDescription();
                }
                bool local1 = (instance != null) || (spell != null);
                if (this.durationVisibility)
                {
                    this.durationVisibility.gameObject.SetActive(instance != null);
                }
                if (this.controllerVisibility)
                {
                    this.controllerVisibility.gameObject.SetActive(instance != null);
                }
                if (this.targetVisibility)
                {
                    this.targetVisibility.gameObject.SetActive((instance != null) || (spell != null));
                }
                if (instance == null)
                {
                    if (this.labelUpkeepCost)
                    {
                        this.labelUpkeepCost.text = source.upkeepCost.ToString();
                    }
                    if ((spell != null) && this.labelTarget)
                    {
                        this.labelTarget.text = TargetTypeExtension.GetLocalizedTargetTypeDescription(spell.targetType.desType);
                    }
                    if (this.durationVisibility)
                    {
                        this.durationVisibility.gameObject.SetActive(source.lifeTime > 0);
                    }
                    if (this.labelDuration)
                    {
                        this.labelDuration.text = source.lifeTime.ToString();
                    }
                }
                else
                {
                    Entity entity;
                    if (instance.owner != null)
                    {
                        entity = instance.owner.GetEntity();
                    }
                    else
                    {
                        Reference owner = instance.owner;
                        entity = null;
                    }
                    PlayerWizard wizard = entity as PlayerWizard;
                    if (this.labelUpkeepCost)
                    {
                        this.labelUpkeepCost.text = instance.GetEnchantmentCost(wizard).ToString();
                    }
                    if (this.labelDuration)
                    {
                        this.labelDuration.text = instance.countDown.ToString();
                    }
                    if (this.durationVisibility)
                    {
                        this.durationVisibility.gameObject.SetActive(instance.countDown > 0);
                    }
                    if (this.controllerVisibility)
                    {
                        this.controllerVisibility.gameObject.SetActive(wizard != null);
                    }
                    if ((wizard != null) && this.labelController)
                    {
                        string str = !wizard.IsHuman ? (GameManager.GetHumanWizard().GetDiscoveredWizards().Contains(wizard) ? wizard.name : DBUtils.Localization.Get("UI_UNKNOWN_WIZARD", true, Array.Empty<object>())) : wizard.name;
                        this.labelController.text = str;
                    }
                    if (this.labelTarget)
                    {
                        this.labelTarget.text = instance.manager.owner.GetName();
                    }
                }
                if (this.labelCanBeDispelled)
                {
                    this.labelCanBeDispelled.text = DBUtils.Localization.Get(source.allowDispel ? "UI_YES" : "UI_NO", true, Array.Empty<object>());
                }
                if (this.realm != null)
                {
                    this.realm.Set(source.realm);
                }
            }
        }
    }
}

