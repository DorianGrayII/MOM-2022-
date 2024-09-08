namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Catches onValueChanged event in a UI Toggle component. Store the new value and/or send events. Event bool data will contain the new Toggle value")]
    public class UiToggleOnValueChangedEvent : ComponentAction<Toggle>
    {
        [RequiredField, CheckForComponent(typeof(Toggle)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Toggle component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;
        [HutongGames.PlayMaker.Tooltip("Send this event when the value changes.")]
        public FsmEvent sendEvent;
        [HutongGames.PlayMaker.Tooltip("Store the new value in bool variable."), UIHint(UIHint.Variable)]
        public FsmBool value;
        private Toggle toggle;

        public void DoOnValueChanged(bool _value)
        {
            this.value.Value = _value;
            Fsm.EventData.BoolData = _value;
            base.SendEvent(this.eventTarget, this.sendEvent);
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!base.UpdateCache(ownerDefaultTarget))
            {
                base.LogError("Missing GameObject");
            }
            else
            {
                if (this.toggle != null)
                {
                    this.toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.DoOnValueChanged));
                }
                this.toggle = base.cachedComponent;
                if (this.toggle != null)
                {
                    this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.DoOnValueChanged));
                }
                else
                {
                    base.LogError("Missing UI.Toggle on " + ownerDefaultTarget.name);
                }
            }
        }

        public override void OnExit()
        {
            if (this.toggle != null)
            {
                this.toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.DoOnValueChanged));
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

