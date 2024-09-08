namespace MOM
{
    using DBDef;
    using DBEnum;
    using DBUtils;
    using MHUtils;
    using MHUtils.UI;
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

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

        public void AdditionalInfiIfValid(Spell spell)
        {
            if (!ReferenceEquals(spell, (Spell) SPELL.GREAT_UNSUMMONING))
            {
                if (ReferenceEquals(spell, (Spell) SPELL.DEATH_WISH) && ((PopupGlobalCast.additionalData != null) && (PopupGlobalCast.additionalData is Multitype<Spell, int>)))
                {
                    Multitype<Spell, int> additionalData = PopupGlobalCast.additionalData as Multitype<Spell, int>;
                    if (spell == additionalData.t0)
                    {
                        object[] parameters = new object[] { additionalData.t1 };
                        string str2 = DBUtils.Localization.Get("UI_DEATH_WISH_OUTCOME", true, parameters);
                        this.labelOwnerName.gameObject.SetActive(true);
                        this.labelOwnerName.text = str2;
                    }
                }
            }
            else if ((PopupGlobalCast.additionalData != null) && (PopupGlobalCast.additionalData is Multitype<Spell, int>))
            {
                Multitype<Spell, int> additionalData = PopupGlobalCast.additionalData as Multitype<Spell, int>;
                if (spell == additionalData.t0)
                {
                    object[] parameters = new object[] { additionalData.t1 };
                    string str = DBUtils.Localization.Get("UI_GREAT_UNSUMMONING_OUTCOME", true, parameters);
                    this.labelOwnerName.gameObject.SetActive(true);
                    this.labelOwnerName.text = str;
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

        public void Set(Spell spell, PlayerWizard caster)
        {
            this.spellItem.Set(spell);
            if (caster == null)
            {
                this.riWizardImage.gameObject.SetActive(false);
                this.labelCasterName.gameObject.SetActive(false);
            }
            else
            {
                this.unknownWizardImage.gameObject.SetActive(false);
                this.riWizardImage.texture = DescriptionInfoExtension.GetTexture(caster.GetBaseWizard());
                object[] parameters = new object[] { caster.GetName() };
                this.labelCasterName.text = DBUtils.Localization.Get("UI_WIZARD_CASTS", true, parameters);
            }
            this.labelTargetName.gameObject.SetActive(false);
            this.labelOwnerName.gameObject.SetActive(false);
            this.riGlobalSpellImage.texture = AssetManager.Get<Texture2D>(spell.additionalGraphic, true);
            this.AdditionalInfiIfValid(spell);
        }

        public void Set(Spell spell, PlayerWizard caster, PlayerWizard target)
        {
            this.spellItem.Set(spell);
            if (caster == null)
            {
                this.riWizardImage.gameObject.SetActive(false);
                this.labelCasterName.gameObject.SetActive(false);
            }
            else
            {
                this.unknownWizardImage.gameObject.SetActive(false);
                this.riWizardImage.texture = DescriptionInfoExtension.GetTexture(caster.GetBaseWizard());
                object[] parameters = new object[] { caster.GetName() };
                this.labelCasterName.text = DBUtils.Localization.Get("UI_WIZARD_CASTS", true, parameters);
            }
            this.labelOwnerName.gameObject.SetActive(false);
            this.labelTargetName.text = DBUtils.Localization.Get("UI_ON " + target.GetName(), true, Array.Empty<object>());
            this.riGlobalSpellImage.gameObject.SetActive(false);
            this.riTargetWizardImage.gameObject.SetActive(true);
            this.riTargetWizardImage.texture = DescriptionInfoExtension.GetTexture(target.GetBaseWizard());
            this.AdditionalInfiIfValid(spell);
        }
    }
}

