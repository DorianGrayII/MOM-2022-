namespace MOM
{
    using MHUtils.UI;
    using System;
    using UnityEngine;

    [RequireComponent(typeof(EnchantmentInfo))]
    public class EnchantmentTooltip : TooltipBase
    {
        private EnchantmentInfo info;
        private bool hideDescription;

        private void Awake()
        {
            this.info = base.GetComponent<EnchantmentInfo>();
        }

        protected override void DoExpand()
        {
            if (base.collapse)
            {
                base.collapse = false;
                this.info.gameObject.SetActive(true);
            }
            base.DoExpand();
        }

        public void HideDescription(bool hide)
        {
            this.hideDescription = hide;
            if (this.info.descriptionVisibility)
            {
                this.info.descriptionVisibility.SetActive(!hide);
            }
        }

        public override void Populate(object o)
        {
            this.info.Set(o);
            this.HideDescription(this.hideDescription);
            ScreenBase.LocalizeTextFields(base.gameObject);
        }
    }
}

