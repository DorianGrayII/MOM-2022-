using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Fires an event when user submits from a UI InputField component. \nThis only fires if the user press Enter, not when field looses focus or user escaped the field.\nEvent string data will contain the text value.")]
    public class UiInputFieldOnSubmitEvent : ComponentAction<InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(InputField))]
        [Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [Tooltip("Send this event when editing ended.")]
        public FsmEvent sendEvent;

        [Tooltip("The content of the InputField when submitting")]
        [UIHint(UIHint.Variable)]
        public FsmString text;

        private InputField inputField;

        public override void Reset()
        {
            this.gameObject = null;
            this.eventTarget = FsmEventTarget.Self;
            this.sendEvent = null;
            this.text = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.inputField = base.cachedComponent;
                if (this.inputField != null)
                {
                    this.inputField.onEndEdit.AddListener(DoOnEndEdit);
                }
            }
        }

        public override void OnExit()
        {
            if (this.inputField != null)
            {
                this.inputField.onEndEdit.RemoveListener(DoOnEndEdit);
            }
        }

        public void DoOnEndEdit(string value)
        {
            if (!this.inputField.wasCanceled)
            {
                this.text.Value = value;
                Fsm.EventData.StringData = value;
                base.SendEvent(this.eventTarget, this.sendEvent);
                base.Finish();
            }
        }
    }
}
