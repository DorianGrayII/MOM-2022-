namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Fires an event when editing ended in a UI InputField component. Event string data will contain the text value, and the boolean will be true is it was a cancel action")]
    public class UiInputFieldOnEndEditEvent : ComponentAction<InputField>
    {
        [RequiredField, CheckForComponent(typeof(InputField)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;
        [HutongGames.PlayMaker.Tooltip("Send this event when editing ended.")]
        public FsmEvent sendEvent;
        [HutongGames.PlayMaker.Tooltip("The content of the InputField when edited ended"), UIHint(UIHint.Variable)]
        public FsmString text;
        [HutongGames.PlayMaker.Tooltip("The canceled state of the InputField when edited ended"), UIHint(UIHint.Variable)]
        public FsmBool wasCanceled;
        private InputField inputField;

        public void DoOnEndEdit(string value)
        {
            this.text.Value = value;
            this.wasCanceled.Value = this.inputField.wasCanceled;
            Fsm.EventData.StringData = value;
            Fsm.EventData.BoolData = this.inputField.wasCanceled;
            base.SendEvent(this.eventTarget, this.sendEvent);
            base.Finish();
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.inputField = base.cachedComponent;
                if (this.inputField != null)
                {
                    this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.DoOnEndEdit));
                }
            }
        }

        public override void OnExit()
        {
            if (this.inputField != null)
            {
                this.inputField.onEndEdit.RemoveListener(new UnityAction<string>(this.DoOnEndEdit));
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.sendEvent = null;
            this.text = null;
            this.wasCanceled = null;
        }
    }
}

