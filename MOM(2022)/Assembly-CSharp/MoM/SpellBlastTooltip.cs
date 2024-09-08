using DBDef;
using DBUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
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
            PlayerWizard playerWizard = o as PlayerWizard;
            Spell curentlyCastSpell = playerWizard.GetMagicAndResearch().curentlyCastSpell;
            if (curentlyCastSpell != null)
            {
                this.goInfo.SetActive(value: true);
                this.goManaInvested.SetActive(value: true);
                this.labelSpellName.text = curentlyCastSpell.GetDescriptionInfo().GetLocalizedName();
                this.labelSpellDescription.text = curentlyCastSpell.GetDescriptionInfo().GetLocalizedDescription();
                this.labelManaInvested.text = playerWizard.GetMagicAndResearch().castingProgress.ToString();
                this.icon.texture = curentlyCastSpell.GetDescriptionInfo().GetTexture();
                this.labelManaInvested1.text = global::DBUtils.Localization.Get("UI_MANA_INVESTED", true);
                this.labelInfo.text = global::DBUtils.Localization.Get("UI_CASTING_SPELL_BLAST_INFO", true);
                if (playerWizard.GetMagicAndResearch().castingProgress > GameManager.GetHumanWizard().mana)
                {
                    this.goInsufficientMana.SetActive(value: true);
                    this.labelInsufficientMana.text = global::DBUtils.Localization.Get("UI_SPELLBLAST_INSUFFICIENT_MANA", true);
                }
            }
            else
            {
                this.labelSpellName.text = global::DBUtils.Localization.Get("UI_WIZARD_NOT_CASTING", true);
                this.labelInfo.text = global::DBUtils.Localization.Get("UI_WIZARD_NOT_CASTING_DES", true);
                this.goInfo.SetActive(value: false);
                this.goManaInvested.SetActive(value: false);
            }
        }
    }
}
