using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets the minimum and maximum limits for the value of a UI Slider component.")]
    public class UiSliderGetMinMax : ComponentAction<Slider>
    {
        [RequiredField]
        [CheckForComponent(typeof(Slider))]
        [Tooltip("The GameObject with the UI Slider component.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the minimum value of the UI Slider.")]
        public FsmFloat minValue;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the maximum value of the UI Slider.")]
        public FsmFloat maxValue;

        private Slider slider;

        public override void Reset()
        {
            this.gameObject = null;
            this.minValue = null;
            this.maxValue = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.slider = base.cachedComponent;
            }
            this.DoGetValue();
        }

        private void DoGetValue()
        {
            if (this.slider != null)
            {
                if (!this.minValue.IsNone)
                {
                    this.minValue.Value = this.slider.minValue;
                }
                if (!this.maxValue.IsNone)
                {
                    this.maxValue.Value = this.slider.maxValue;
                }
            }
        }
    }
}
