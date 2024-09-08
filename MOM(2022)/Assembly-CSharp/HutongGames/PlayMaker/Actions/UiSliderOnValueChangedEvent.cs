namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Catches onValueChanged event for a UI Slider component. Store the new value and/or send events. Event float data will contain the new slider value")]
    public class UiSliderOnValueChangedEvent : ComponentAction<Slider>
    {
        [RequiredField, CheckForComponent(typeof(Slider)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Slider component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;
        [HutongGames.PlayMaker.Tooltip("Send this event when Clicked.")]
        public FsmEvent sendEvent;
        [HutongGames.PlayMaker.Tooltip("Store the new value in float variable."), UIHint(UIHint.Variable)]
        public FsmFloat value;
        private Slider slider;

        public void DoOnValueChanged(float _value)
        {
            this.value.Value = _value;
            Fsm.EventData.FloatData = _value;
            base.SendEvent(this.eventTarget, this.sendEvent);
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.slider = base.cachedComponent;
                if (this.slider != null)
                {
                    this.slider.onValueChanged.AddListener(new UnityAction<float>(this.DoOnValueChanged));
                }
            }
        }

        public override void OnExit()
        {
            if (this.slider != null)
            {
                this.slider.onValueChanged.RemoveListener(new UnityAction<float>(this.DoOnValueChanged));
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.eventTarget = FsmEventTarget.Self;
            this.sendEvent = null;
            this.value = null;
        }
    }
}

