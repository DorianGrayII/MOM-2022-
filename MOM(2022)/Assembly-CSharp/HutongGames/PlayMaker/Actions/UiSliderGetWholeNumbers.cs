using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets the wholeNumbers property of a UI Slider component. If true, the Slider is constrained to integer values")]
    public class UiSliderGetWholeNumbers : ComponentAction<Slider>
    {
        [RequiredField]
        [CheckForComponent(typeof(Slider))]
        [Tooltip("The GameObject with the UI Slider component.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Is the Slider constrained to integer values?")]
        public FsmBool wholeNumbers;

        [Tooltip("Event sent if slider is showing integers")]
        public FsmEvent isShowingWholeNumbersEvent;

        [Tooltip("Event sent if slider is showing floats")]
        public FsmEvent isNotShowingWholeNumbersEvent;

        private Slider slider;

        public override void Reset()
        {
            this.gameObject = null;
            this.isShowingWholeNumbersEvent = null;
            this.isNotShowingWholeNumbersEvent = null;
            this.wholeNumbers = null;
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

        private void DoGetValue()
        {
            bool flag = false;
            if (this.slider != null)
            {
                flag = this.slider.wholeNumbers;
            }
            this.wholeNumbers.Value = flag;
            base.Fsm.Event(flag ? this.isShowingWholeNumbersEvent : this.isNotShowingWholeNumbersEvent);
        }
    }
}
