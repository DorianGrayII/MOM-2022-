using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Fires an event when editing ended in a UI InputField component. Event string data will contain the text value, and the boolean will be true is it was a cancel action")]
    public class UiInputFieldOnEndEditEvent : ComponentAction<InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(InputField))]
        [Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [Tooltip("Send this event when editing ended.")]
        public FsmEvent sendEvent;

        [Tooltip("The content of the InputField when edited ended")]
        [UIHint(UIHint.Variable)]
        public FsmString text;

        [Tooltip("The canceled state of the InputField when edited ended")]
        [UIHint(UIHint.Variable)]
        public FsmBool wasCanceled;

        private InputField inputField;

        public override void Reset()
        {
            this.gameObject = null;
            this.sendEvent = null;
            this.text = null;
            this.wasCanceled = null;
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
            this.text.Value = value;
            this.wasCanceled.Value = this.inputField.wasCanceled;
            Fsm.EventData.StringData = value;
            Fsm.EventData.BoolData = this.inputField.wasCanceled;
            base.SendEvent(this.eventTarget, this.sendEvent);
            base.Finish();
        }
    }
}
