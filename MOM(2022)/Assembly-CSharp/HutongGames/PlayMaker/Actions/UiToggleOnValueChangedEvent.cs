using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Catches onValueChanged event in a UI Toggle component. Store the new value and/or send events. Event bool data will contain the new Toggle value")]
    public class UiToggleOnValueChangedEvent : ComponentAction<Toggle>
    {
        [RequiredField]
        [CheckForComponent(typeof(Toggle))]
        [Tooltip("The GameObject with the UI Toggle component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [Tooltip("Send this event when the value changes.")]
        public FsmEvent sendEvent;

        [Tooltip("Store the new value in bool variable.")]
        [UIHint(UIHint.Variable)]
        public FsmBool value;

        private Toggle toggle;

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
                if (this.toggle != null)
                {
                    this.toggle.onValueChanged.RemoveListener(DoOnValueChanged);
                }
                this.toggle = base.cachedComponent;
                if (this.toggle != null)
                {
                    this.toggle.onValueChanged.AddListener(DoOnValueChanged);
                }
                else
                {
                    base.LogError("Missing UI.Toggle on " + ownerDefaultTarget.name);
                }
            }
            else
            {
                base.LogError("Missing GameObject");
            }
        }

        public override void OnExit()
        {
            if (this.toggle != null)
            {
                this.toggle.onValueChanged.RemoveListener(DoOnValueChanged);
            }
        }

        public void DoOnValueChanged(bool _value)
        {
            this.value.Value = _value;
            Fsm.EventData.BoolData = _value;
            base.SendEvent(this.eventTarget, this.sendEvent);
        }
    }
}
