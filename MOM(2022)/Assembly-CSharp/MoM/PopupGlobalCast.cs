using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class PopupGlobalCast : ScreenBase
    {
        public Button btOkay;

        public RawImage riWizardImage;

        public RawImage riTargetWizardImage;

        public RawImage riGlobalSpellImage;

        public GameObject unknownWizardImage;

        public TextMeshProUGUI labelCasterName;

        public TextMeshProUGUI labelTargetName;

        public TextMeshProUGUI labelOwnerName;

        public SpellItem spellItem;

        public static object additionalData;

        public void Set(Spell spell, PlayerWizard caster)
        {
            this.spellItem.Set(spell);
            if (caster != null)
            {
                this.unknownWizardImage.gameObject.SetActive(value: false);
                this.riWizardImage.texture = caster.GetBaseWizard().GetTexture();
                this.labelCasterName.text = global::DBUtils.Localization.Get("UI_WIZARD_CASTS", true, caster.GetName());
            }
            else
            {
                this.riWizardImage.gameObject.SetActive(value: false);
                this.labelCasterName.gameObject.SetActive(value: false);
            }
            this.labelTargetName.gameObject.SetActive(value: false);
            this.labelOwnerName.gameObject.SetActive(value: false);
            this.riGlobalSpellImage.texture = AssetManager.Get<Texture2D>(spell.additionalGraphic);
            this.AdditionalInfiIfValid(spell);
        }

        public void Set(Spell spell, PlayerWizard caster, PlayerWizard target)
        {
            this.spellItem.Set(spell);
            if (caster != null)
            {
                this.unknownWizardImage.gameObject.SetActive(value: false);
                this.riWizardImage.texture = caster.GetBaseWizard().GetTexture();
                this.labelCasterName.text = global::DBUtils.Localization.Get("UI_WIZARD_CASTS", true, caster.GetName());
            }
            else
            {
                this.riWizardImage.gameObject.SetActive(value: false);
                this.labelCasterName.gameObject.SetActive(value: false);
            }
            this.labelOwnerName.gameObject.SetActive(value: false);
            this.labelTargetName.text = global::DBUtils.Localization.Get("UI_ON " + target.GetName(), true);
            this.riGlobalSpellImage.gameObject.SetActive(value: false);
            this.riTargetWizardImage.gameObject.SetActive(value: true);
            this.riTargetWizardImage.texture = target.GetBaseWizard().GetTexture();
            this.AdditionalInfiIfValid(spell);
        }

        public void AdditionalInfiIfValid(Spell spell)
        {
            if (spell == (Spell)SPELL.GREAT_UNSUMMONING)
            {
                if (PopupGlobalCast.additionalData != null && PopupGlobalCast.additionalData is Multitype<Spell, int>)
                {
                    Multitype<Spell, int> multitype = PopupGlobalCast.additionalData as Multitype<Spell, int>;
                    if (spell == multitype.t0)
                    {
                        string text = global::DBUtils.Localization.Get("UI_GREAT_UNSUMMONING_OUTCOME", true, multitype.t1);
                        this.labelOwnerName.gameObject.SetActive(value: true);
                        this.labelOwnerName.text = text;
                    }
                }
            }
            else if (spell == (Spell)SPELL.DEATH_WISH && PopupGlobalCast.additionalData != null && PopupGlobalCast.additionalData is Multitype<Spell, int>)
            {
                Multitype<Spell, int> multitype2 = PopupGlobalCast.additionalData as Multitype<Spell, int>;
                if (spell == multitype2.t0)
                {
                    string text2 = global::DBUtils.Localization.Get("UI_DEATH_WISH_OUTCOME", true, multitype2.t1);
                    this.labelOwnerName.gameObject.SetActive(value: true);
                    this.labelOwnerName.text = text2;
                }
            }
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.btOkay)
            {
                UIManager.Close(this);
            }
        }
    }
}
