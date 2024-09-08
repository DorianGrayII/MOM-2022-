using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the minimum and maximum limits for the value of a UI Slider component. Optionally resets on exit")]
    public class UiSliderSetMinMax : ComponentAction<Slider>
    {
        [RequiredField]
        [CheckForComponent(typeof(Slider))]
        [Tooltip("The GameObject with the UI Slider component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The minimum value of the UI Slider component. Leave as None for no effect")]
        public FsmFloat minValue;

        [Tooltip("The maximum value of the UI Slider component. Leave as None for no effect")]
        public FsmFloat maxValue;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private Slider slider;

        private float originalMinValue;

        private float originalMaxValue;

        public override void Reset()
        {
            this.gameObject = null;
            this.minValue = new FsmFloat
            {
                UseVariable = true
            };
            this.maxValue = new FsmFloat
            {
                UseVariable = true
            };
            this.resetOnExit = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.slider = base.cachedComponent;
            }
            if (this.resetOnExit.Value)
            {
                this.originalMinValue = this.slider.minValue;
                this.originalMaxValue = this.slider.maxValue;
            }
            this.DoSetValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetValue();
        }

        private void DoSetValue()
        {
            if (!(this.slider == null))
            {
                if (!this.minValue.IsNone)
                {
                    this.slider.minValue = this.minValue.Value;
                }
                if (!this.maxValue.IsNone)
                {
                    this.slider.maxValue = this.maxValue.Value;
                }
            }
        }

        public override void OnExit()
        {
            if (!(this.slider == null) && this.resetOnExit.Value)
            {
                this.slider.minValue = this.originalMinValue;
                this.slider.maxValue = this.originalMaxValue;
            }
        }
    }
}
