using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Catches UI InputField onValueChanged event. Store the new value and/or send events. Event string data also contains the new value.")]
    public class UiInputFieldOnValueChangeEvent : ComponentAction<InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(InputField))]
        [Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [Tooltip("Send this event when value changed.")]
        public FsmEvent sendEvent;

        [Tooltip("Store new value in string variable.")]
        [UIHint(UIHint.Variable)]
        public FsmString text;

        private InputField inputField;

        public override void Reset()
        {
            this.gameObject = null;
            this.text = null;
            this.eventTarget = FsmEventTarget.Self;
            this.sendEvent = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.inputField = base.cachedComponent;
                if (this.inputField != null)
                {
                    this.inputField.onValueChanged.AddListener(DoOnValueChange);
                }
            }
        }

        public override void OnExit()
        {
            if (this.inputField != null)
            {
                this.inputField.onValueChanged.RemoveListener(DoOnValueChange);
            }
        }

        public void DoOnValueChange(string value)
        {
            this.text.Value = value;
            Fsm.EventData.StringData = value;
            base.SendEvent(this.eventTarget, this.sendEvent);
        }
    }
}
