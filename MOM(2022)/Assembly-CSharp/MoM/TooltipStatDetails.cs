using System.Collections.Generic;
using System.Linq;
using DBDef;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class TooltipStatDetails : TooltipBase
    {
        public TextMeshProUGUI labelStatName;

        public TextMeshProUGUI labelStatDescription;

        public TextMeshProUGUI labelTotalValue;

        public StatModifierListItem listItem;

        public RawImage icon;

        public List<StatModifierListItem> resources;

        public void Awake()
        {
            ScreenBase.LocalizeTextFields(base.gameObject);
        }

        public override void Populate(object source)
        {
            StatDetails statDetails = source as StatDetails;
            RolloverObject rolloverObject = TooltipBase.rollOver as RolloverObject;
            this.icon.texture = rolloverObject.overrideTexture;
            this.labelStatDescription.text = global::DBUtils.Localization.Get(rolloverObject.overrideDescription, true);
            this.labelStatName.text = global::DBUtils.Localization.Get(rolloverObject.overrideTitle, true);
            List<string> list = statDetails.breakdown.Keys.ToList();
            FInt zERO = FInt.ZERO;
            foreach (string item in list)
            {
                StatModifierListItem component = Object.Instantiate(this.listItem.gameObject, this.listItem.transform.parent).GetComponent<StatModifierListItem>();
                DBClass dBClass = DataBase.Get(item, reportMissing: false);
                if (dBClass != null)
                {
                    component.labelName.text = ((IDescriptionInfoType)dBClass).GetDILocalizedName();
                }
                else
                {
                    component.labelName.text = global::DBUtils.Localization.Get(item, true);
                }
                if (statDetails.breakdown.ContainsKey(item))
                {
                    FInt fInt = statDetails.breakdown[item];
                    zERO += fInt;
                    component.labelValue.text = fInt.ToStringTryInt();
                }
            }
            this.listItem.gameObject.SetActive(value: false);
            this.labelTotalValue.text = zERO.ToStringTryInt();
        }
    }
}
