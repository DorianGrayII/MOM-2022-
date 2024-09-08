namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the wholeNumbers property of a UI Slider component. This defines if the slider will be constrained to integer values ")]
    public class UiSliderSetWholeNumbers : ComponentAction<Slider>
    {
        [RequiredField, CheckForComponent(typeof(Slider)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Slider component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Should the slider be constrained to integer values?")]
        public FsmBool wholeNumbers;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        private Slider slider;
        private bool originalValue;

        private void DoSetValue()
        {
            if (this.slider != null)
            {
                this.slider.wholeNumbers = this.wholeNumbers.Value;
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.slider = base.cachedComponent;
            }
            this.originalValue = this.slider.wholeNumbers;
            this.DoSetValue();
            base.Finish();
        }

        public override void OnExit()
        {
            if ((this.slider != null) && this.resetOnExit.Value)
            {
                this.slider.wholeNumbers = this.originalValue;
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.wholeNumbers = null;
            this.resetOnExit = null;
        }
    }
}

