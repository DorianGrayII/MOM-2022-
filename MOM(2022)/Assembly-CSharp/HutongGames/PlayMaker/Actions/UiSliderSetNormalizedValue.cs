namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the normalized value ( between 0 and 1) of a UI Slider component.")]
    public class UiSliderSetNormalizedValue : ComponentAction<Slider>
    {
        [RequiredField, CheckForComponent(typeof(Slider)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Slider component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HasFloatSlider(0f, 1f), HutongGames.PlayMaker.Tooltip("The normalized value ( between 0 and 1) of the UI Slider component.")]
        public FsmFloat value;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame")]
        public bool everyFrame;
        private Slider slider;
        private float originalValue;

        private void DoSetValue()
        {
            if (this.slider != null)
            {
                this.slider.normalizedValue = this.value.Value;
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.slider = base.cachedComponent;
            }
            this.originalValue = this.slider.normalizedValue;
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
                this.slider.normalizedValue = this.originalValue;
            }
        }

        public override void OnUpdate()
        {
            this.DoSetValue();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.value = null;
            this.resetOnExit = null;
            this.everyFrame = false;
        }
    }
}

