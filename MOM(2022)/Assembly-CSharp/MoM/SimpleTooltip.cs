namespace MOM
{
    using DBDef;
    using DBUtils;
    using MHUtils;
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SimpleTooltip : TooltipBase
    {
        public TextMeshProUGUI labelName;
        public TextMeshProUGUI labelDescription;
        public RawImage icon;
        public GameObject infoBackground;
        public GameObject info;
        public GameObject graphicWrapper;
        public GameObject thisTooltip;
        private bool haveInfo;

        protected override void DoExpand()
        {
            if (this.haveInfo)
            {
                this.infoBackground.SetActive(true);
                this.info.SetActive(true);
            }
            base.DoExpand();
        }

        private string GetDynamicParameter(GameObject go)
        {
            Settings.KeyActions actions;
            UIKeyboardClick component = go.GetComponent<UIKeyboardClick>();
            if ((component != null) && (component.action != Settings.KeyActions.None))
            {
                return SettingsBlock.GetKeyForAction(component.action).ToString();
            }
            RolloverSimpleTooltip tooltip = go.GetComponent<RolloverSimpleTooltip>();
            if ((tooltip == null) || string.IsNullOrEmpty(tooltip.data))
            {
                return "";
            }
            Enum.TryParse<Settings.KeyActions>(tooltip.data, out actions);
            return SettingsBlock.GetKeyForAction(actions).ToString();
        }

        public override void Populate(object o)
        {
            Texture2D icon = null;
            this.haveInfo = false;
            RolloverSimpleTooltip tooltip = o as RolloverSimpleTooltip;
            if (tooltip != null)
            {
                object[] parameters = new object[] { this.GetDynamicParameter(tooltip.gameObject) };
                this.labelName.text = DBUtils.Localization.Get(tooltip.GetTitle(), true, parameters);
                string description = tooltip.GetDescription();
                if (string.IsNullOrEmpty(description))
                {
                    this.thisTooltip.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300f);
                }
                else
                {
                    this.haveInfo = true;
                    this.labelDescription.text = DBUtils.Localization.Get(description, true, Array.Empty<object>());
                    icon = tooltip.GetIcon();
                }
            }
            else
            {
                RolloverObject obj2 = o as RolloverObject;
                if (obj2 != null)
                {
                    this.labelName.text = DBUtils.Localization.Get(obj2.overrideTitle, true, Array.Empty<object>());
                    this.labelDescription.text = DBUtils.Localization.Get(obj2.overrideDescription, true, Array.Empty<object>());
                    this.haveInfo = !string.IsNullOrEmpty(this.labelDescription.text);
                    icon = obj2.overrideTexture;
                }
                else
                {
                    string instanceName = o as string;
                    if (instanceName != null)
                    {
                        o = DataBase.Get(instanceName, true);
                    }
                    DescriptionInfo descriptionInfo = o as DescriptionInfo;
                    if ((descriptionInfo == null) && (o is IDescriptionInfoType))
                    {
                        descriptionInfo = (o as IDescriptionInfoType).GetDescriptionInfo();
                    }
                    if (descriptionInfo != null)
                    {
                        this.labelName.text = descriptionInfo.GetLocalizedName();
                    }
                    PlayerWizard wizard = o as PlayerWizard;
                    if (wizard != null)
                    {
                        descriptionInfo = wizard.GetBaseWizard().GetDescriptionInfo();
                        this.labelName.text = wizard.name;
                    }
                    if (descriptionInfo != null)
                    {
                        this.haveInfo = true;
                        this.labelDescription.text = descriptionInfo.GetLocalizedDescription();
                        icon = AssetManager.Get<Texture2D>(descriptionInfo.graphic, true);
                    }
                    else
                    {
                        MOM.Artefact artefact = o as MOM.Artefact;
                        if (artefact != null)
                        {
                            this.haveInfo = true;
                            this.labelName.text = artefact.name;
                            this.labelDescription.text = artefact.localizedDescription;
                            icon = AssetManager.Get<Texture2D>(artefact.graphic, true);
                        }
                    }
                }
            }
            this.info.SetActive(this.haveInfo && !base.collapse);
            this.infoBackground.SetActive(this.info.activeSelf);
            this.graphicWrapper.SetActive(icon != null);
            this.icon.texture = icon;
        }
    }
}

