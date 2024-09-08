namespace MOM
{
    using DBDef;
    using DBUtils;
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SpellBlastTooltip : TooltipBase
    {
        public TextMeshProUGUI labelSpellName;
        public TextMeshProUGUI labelSpellDescription;
        public TextMeshProUGUI labelManaInvested;
        public TextMeshProUGUI labelManaInvested1;
        public TextMeshProUGUI labelInfo;
        public TextMeshProUGUI labelInsufficientMana;
        public GameObject goInfo;
        public GameObject goManaInvested;
        public GameObject goInsufficientMana;
        public RawImage icon;

        public override void Populate(object o)
        {
            PlayerWizard wizard = o as PlayerWizard;
            Spell curentlyCastSpell = wizard.GetMagicAndResearch().curentlyCastSpell;
            if (curentlyCastSpell == null)
            {
                this.labelSpellName.text = DBUtils.Localization.Get("UI_WIZARD_NOT_CASTING", true, Array.Empty<object>());
                this.labelInfo.text = DBUtils.Localization.Get("UI_WIZARD_NOT_CASTING_DES", true, Array.Empty<object>());
                this.goInfo.SetActive(false);
                this.goManaInvested.SetActive(false);
            }
            else
            {
                this.goInfo.SetActive(true);
                this.goManaInvested.SetActive(true);
                this.labelSpellName.text = curentlyCastSpell.GetDescriptionInfo().GetLocalizedName();
                this.labelSpellDescription.text = curentlyCastSpell.GetDescriptionInfo().GetLocalizedDescription();
                this.labelManaInvested.text = wizard.GetMagicAndResearch().castingProgress.ToString();
                this.icon.texture = DescriptionInfoExtension.GetTexture(curentlyCastSpell.GetDescriptionInfo());
                this.labelManaInvested1.text = DBUtils.Localization.Get("UI_MANA_INVESTED", true, Array.Empty<object>());
                this.labelInfo.text = DBUtils.Localization.Get("UI_CASTING_SPELL_BLAST_INFO", true, Array.Empty<object>());
                if (wizard.GetMagicAndResearch().castingProgress > GameManager.GetHumanWizard().mana)
                {
                    this.goInsufficientMana.SetActive(true);
                    this.labelInsufficientMana.text = DBUtils.Localization.Get("UI_SPELLBLAST_INSUFFICIENT_MANA", true, Array.Empty<object>());
                }
            }
        }
    }
}

