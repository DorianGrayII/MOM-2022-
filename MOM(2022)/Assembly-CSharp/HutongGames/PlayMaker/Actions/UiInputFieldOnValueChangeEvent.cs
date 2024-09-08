namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Catches UI InputField onValueChanged event. Store the new value and/or send events. Event string data also contains the new value.")]
    public class UiInputFieldOnValueChangeEvent : ComponentAction<InputField>
    {
        [RequiredField, CheckForComponent(typeof(InputField)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;
        [HutongGames.PlayMaker.Tooltip("Send this event when value changed.")]
        public FsmEvent sendEvent;
        [HutongGames.PlayMaker.Tooltip("Store new value in string variable."), UIHint(UIHint.Variable)]
        public FsmString text;
        private InputField inputField;

        public void DoOnValueChange(string value)
        {
            this.text.Value = value;
            Fsm.EventData.StringData = value;
            base.SendEvent(this.eventTarget, this.sendEvent);
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.inputField = base.cachedComponent;
                if (this.inputField != null)
                {
                    this.inputField.onValueChanged.AddListener(new UnityAction<string>(this.DoOnValueChange));
                }
            }
        }

        public override void OnExit()
        {
            if (this.inputField != null)
            {
                this.inputField.onValueChanged.RemoveListener(new UnityAction<string>(this.DoOnValueChange));
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.text = null;
            this.eventTarget = FsmEventTarget.Self;
            this.sendEvent = null;
        }
    }
}

