using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Catches onValueChanged event for a UI Slider component. Store the new value and/or send events. Event float data will contain the new slider value")]
    public class UiSliderOnValueChangedEvent : ComponentAction<Slider>
    {
        [RequiredField]
        [CheckForComponent(typeof(Slider))]
        [Tooltip("The GameObject with the UI Slider component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [Tooltip("Send this event when Clicked.")]
        public FsmEvent sendEvent;

        [Tooltip("Store the new value in float variable.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat value;

        private Slider slider;

        public override void Reset()
        {
            this.gameObject = null;
            this.eventTarget = FsmEventTarget.Self;
            this.sendEvent = null;
            this.value = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.slider = base.cachedComponent;
                if (this.slider != null)
                {
                    this.slider.onValueChanged.AddListener(DoOnValueChanged);
                }
            }
        }

        public override void OnExit()
        {
            if (this.slider != null)
            {
                this.slider.onValueChanged.RemoveListener(DoOnValueChanged);
            }
        }

        public void DoOnValueChanged(float _value)
        {
            this.value.Value = _value;
            Fsm.EventData.FloatData = _value;
            base.SendEvent(this.eventTarget, this.sendEvent);
        }
    }
}
