namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the minimum and maximum limits for the value of a UI Slider component. Optionally resets on exit")]
    public class UiSliderSetMinMax : ComponentAction<Slider>
    {
        [RequiredField, CheckForComponent(typeof(Slider)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Slider component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The minimum value of the UI Slider component. Leave as None for no effect")]
        public FsmFloat minValue;
        [HutongGames.PlayMaker.Tooltip("The maximum value of the UI Slider component. Leave as None for no effect")]
        public FsmFloat maxValue;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame")]
        public bool everyFrame;
        private Slider slider;
        private float originalMinValue;
        private float originalMaxValue;

        private void DoSetValue()
        {
            if (this.slider != null)
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

        public override void OnExit()
        {
            if ((this.slider != null) && this.resetOnExit.Value)
            {
                this.slider.minValue = this.originalMinValue;
                this.slider.maxValue = this.originalMaxValue;
            }
        }

        public override void OnUpdate()
        {
            this.DoSetValue();
        }

        public override void Reset()
        {
            this.gameObject = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.minValue = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.maxValue = num2;
            this.resetOnExit = null;
            this.everyFrame = false;
        }
    }
}

