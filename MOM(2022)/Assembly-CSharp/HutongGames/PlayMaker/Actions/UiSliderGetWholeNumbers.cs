namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Gets the wholeNumbers property of a UI Slider component. If true, the Slider is constrained to integer values")]
    public class UiSliderGetWholeNumbers : ComponentAction<Slider>
    {
        [RequiredField, CheckForComponent(typeof(Slider)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Slider component.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Is the Slider constrained to integer values?")]
        public FsmBool wholeNumbers;
        [HutongGames.PlayMaker.Tooltip("Event sent if slider is showing integers")]
        public FsmEvent isShowingWholeNumbersEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if slider is showing floats")]
        public FsmEvent isNotShowingWholeNumbersEvent;
        private Slider slider;

        private void DoGetValue()
        {
            bool wholeNumbers = false;
            if (this.slider != null)
            {
                wholeNumbers = this.slider.wholeNumbers;
            }
            this.wholeNumbers.Value = wholeNumbers;
            base.Fsm.Event(wholeNumbers ? this.isShowingWholeNumbersEvent : this.isNotShowingWholeNumbersEvent);
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.slider = base.cachedComponent;
            }
            this.DoGetValue();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.isShowingWholeNumbersEvent = null;
            this.isNotShowingWholeNumbersEvent = null;
            this.wholeNumbers = null;
        }
    }
}

