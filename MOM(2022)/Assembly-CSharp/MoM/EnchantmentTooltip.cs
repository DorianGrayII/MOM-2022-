using MHUtils.UI;
using UnityEngine;

namespace MOM
{
    [RequireComponent(typeof(EnchantmentInfo))]
    public class EnchantmentTooltip : TooltipBase
    {
        private EnchantmentInfo info;

        private bool hideDescription;

        private void Awake()
        {
            this.info = base.GetComponent<EnchantmentInfo>();
        }

        public override void Populate(object o)
        {
            this.info.Set(o);
            this.HideDescription(this.hideDescription);
            ScreenBase.LocalizeTextFields(base.gameObject);
        }

        public void HideDescription(bool hide)
        {
            this.hideDescription = hide;
            if ((bool)this.info.descriptionVisibility)
            {
                this.info.descriptionVisibility.SetActive(!hide);
            }
        }

        protected override void DoExpand()
        {
            if (base.collapse)
            {
                base.collapse = false;
                this.info.gameObject.SetActive(value: true);
            }
            base.DoExpand();
        }
    }
}
