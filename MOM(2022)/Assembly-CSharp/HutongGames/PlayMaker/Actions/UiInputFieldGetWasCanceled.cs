namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Gets the cancel state of a UI InputField component. This relates to the last onEndEdit Event")]
    public class UiInputFieldGetWasCanceled : ComponentAction<InputField>
    {
        [RequiredField, CheckForComponent(typeof(InputField)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The was canceled flag value of the UI InputField component.")]
        public FsmBool wasCanceled;
        [HutongGames.PlayMaker.Tooltip("Event sent if inputField was canceled")]
        public FsmEvent wasCanceledEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if inputField was not canceled")]
        public FsmEvent wasNotCanceledEvent;
        private InputField inputField;

        private void DoGetValue()
        {
            if (this.inputField != null)
            {
                this.wasCanceled.Value = this.inputField.wasCanceled;
                base.Fsm.Event(this.inputField.wasCanceled ? this.wasCanceledEvent : this.wasNotCanceledEvent);
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.inputField = base.cachedComponent;
            }
            this.DoGetValue();
            base.Finish();
        }

        public override void Reset()
        {
            this.wasCanceled = null;
            this.wasCanceledEvent = null;
            this.wasNotCanceledEvent = null;
        }
    }
}

