using System;
using DBDef;
using DBUtils;
using MHUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
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
                this.infoBackground.SetActive(value: true);
                this.info.SetActive(value: true);
            }
            base.DoExpand();
        }

        public override void Populate(object o)
        {
            Texture2D texture2D = null;
            this.haveInfo = false;
            if (o is RolloverSimpleTooltip rolloverSimpleTooltip)
            {
                this.labelName.text = global::DBUtils.Localization.Get(rolloverSimpleTooltip.GetTitle(), true, this.GetDynamicParameter(rolloverSimpleTooltip.gameObject));
                string description = rolloverSimpleTooltip.GetDescription();
                if (string.IsNullOrEmpty(description))
                {
                    this.thisTooltip.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300f);
                }
                else
                {
                    this.haveInfo = true;
                    this.labelDescription.text = global::DBUtils.Localization.Get(description, true);
                    texture2D = rolloverSimpleTooltip.GetIcon();
                }
            }
            else if (o is RolloverObject rolloverObject)
            {
                this.labelName.text = global::DBUtils.Localization.Get(rolloverObject.overrideTitle, true);
                this.labelDescription.text = global::DBUtils.Localization.Get(rolloverObject.overrideDescription, true);
                this.haveInfo = !string.IsNullOrEmpty(this.labelDescription.text);
                texture2D = rolloverObject.overrideTexture;
            }
            else
            {
                if (o is string instanceName)
                {
                    o = DataBase.Get(instanceName, reportMissing: true);
                }
                DescriptionInfo descriptionInfo = o as DescriptionInfo;
                if (descriptionInfo == null && o is IDescriptionInfoType)
                {
                    descriptionInfo = (o as IDescriptionInfoType).GetDescriptionInfo();
                }
                if (descriptionInfo != null)
                {
                    this.labelName.text = descriptionInfo.GetLocalizedName();
                }
                if (o is PlayerWizard playerWizard)
                {
                    descriptionInfo = playerWizard.GetBaseWizard().GetDescriptionInfo();
                    this.labelName.text = playerWizard.name;
                }
                if (descriptionInfo != null)
                {
                    this.haveInfo = true;
                    this.labelDescription.text = descriptionInfo.GetLocalizedDescription();
                    texture2D = AssetManager.Get<Texture2D>(descriptionInfo.graphic);
                }
                else if (o is Artefact artefact)
                {
                    this.haveInfo = true;
                    this.labelName.text = artefact.name;
                    this.labelDescription.text = artefact.localizedDescription;
                    texture2D = AssetManager.Get<Texture2D>(artefact.graphic);
                }
            }
            this.info.SetActive(this.haveInfo && !base.collapse);
            this.infoBackground.SetActive(this.info.activeSelf);
            this.graphicWrapper.SetActive(texture2D != null);
            this.icon.texture = texture2D;
        }

        private string GetDynamicParameter(GameObject go)
        {
            UIKeyboardClick component = go.GetComponent<UIKeyboardClick>();
            if (component != null && component.action != 0)
            {
                return SettingsBlock.GetKeyForAction(component.action).ToString();
            }
            RolloverSimpleTooltip component2 = go.GetComponent<RolloverSimpleTooltip>();
            if (component2 != null && !string.IsNullOrEmpty(component2.data))
            {
                Enum.TryParse<Settings.KeyActions>(component2.data, out var result);
                return SettingsBlock.GetKeyForAction(result).ToString();
            }
            return "";
        }
    }
}
