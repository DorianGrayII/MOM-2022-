namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Gets the Hide Mobile Input value of a UI InputField component.")]
    public class UiInputFieldGetHideMobileInput : ComponentAction<InputField>
    {
        [RequiredField, CheckForComponent(typeof(InputField)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the Hide Mobile flag value of the UI InputField component.")]
        public FsmBool hideMobileInput;
        [HutongGames.PlayMaker.Tooltip("Event sent if hide mobile input property is true")]
        public FsmEvent mobileInputHiddenEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if hide mobile input property is false")]
        public FsmEvent mobileInputShownEvent;
        private InputField inputField;

        private void DoGetValue()
        {
            if (this.inputField != null)
            {
                this.hideMobileInput.Value = this.inputField.shouldHideMobileInput;
                base.Fsm.Event(this.inputField.shouldHideMobileInput ? this.mobileInputHiddenEvent : this.mobileInputShownEvent);
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
            this.hideMobileInput = null;
            this.mobileInputHiddenEvent = null;
            this.mobileInputShownEvent = null;
        }
    }
}

