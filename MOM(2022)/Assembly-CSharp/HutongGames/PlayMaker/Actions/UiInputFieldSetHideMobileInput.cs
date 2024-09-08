namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the Hide Mobile Input property of a UI InputField component.")]
    public class UiInputFieldSetHideMobileInput : ComponentAction<InputField>
    {
        [RequiredField, CheckForComponent(typeof(InputField)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.TextArea), HutongGames.PlayMaker.Tooltip("The Hide Mobile Input flag value of the UI InputField component.")]
        public FsmBool hideMobileInput;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        private InputField inputField;
        private bool originalValue;

        private void DoSetValue()
        {
            if (this.inputField != null)
            {
                this.inputField.shouldHideMobileInput = this.hideMobileInput.Value;
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.inputField = base.cachedComponent;
            }
            this.originalValue = this.inputField.shouldHideMobileInput;
            this.DoSetValue();
            base.Finish();
        }

        public override void OnExit()
        {
            if ((this.inputField != null) && this.resetOnExit.Value)
            {
                this.inputField.shouldHideMobileInput = this.originalValue;
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.hideMobileInput = null;
            this.resetOnExit = null;
        }
    }
}

