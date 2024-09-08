namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Fires an event when user submits from a UI InputField component. \nThis only fires if the user press Enter, not when field looses focus or user escaped the field.\nEvent string data will contain the text value.")]
    public class UiInputFieldOnSubmitEvent : ComponentAction<InputField>
    {
        [RequiredField, CheckForComponent(typeof(InputField)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;
        [HutongGames.PlayMaker.Tooltip("Send this event when editing ended.")]
        public FsmEvent sendEvent;
        [HutongGames.PlayMaker.Tooltip("The content of the InputField when submitting"), UIHint(UIHint.Variable)]
        public FsmString text;
        private InputField inputField;

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
            this.eventTarget = FsmEventTarget.Self;
            this.sendEvent = null;
            this.text = null;
        }
    }
}

