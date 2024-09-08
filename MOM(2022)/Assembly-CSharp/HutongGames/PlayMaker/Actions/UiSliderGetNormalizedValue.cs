using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets the normalized value (between 0 and 1) of a UI Slider component.")]
    public class UiSliderGetNormalizedValue : ComponentAction<Slider>
    {
        [RequiredField]
        [CheckForComponent(typeof(Slider))]
        [Tooltip("The GameObject with the UI Slider component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The normalized value (between 0 and 1) of the UI Slider.")]
        public FsmFloat value;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private Slider slider;

        public override void Reset()
        {
            this.gameObject = null;
            this.value = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.slider = base.cachedComponent;
            }
            this.DoGetValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetValue();
        }

        private void DoGetValue()
        {
            if (this.slider != null)
            {
                this.value.Value = this.slider.normalizedValue;
            }
        }
    }
}
