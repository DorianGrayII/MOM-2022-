namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the Asterix Character of a UI InputField component.")]
    public class UiInputFieldSetAsterixChar : ComponentAction<InputField>
    {
        [RequiredField, CheckForComponent(typeof(InputField)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The asterix Character used for password field type of the UI InputField component. Only the first character will be used, the rest of the string will be ignored")]
        public FsmString asterixChar;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        private InputField inputField;
        private char originalValue;
        private static char __char__ = ' ';

        private void DoSetValue()
        {
            char ch = __char__;
            if (this.asterixChar.Value.Length > 0)
            {
                ch = this.asterixChar.Value[0];
            }
            if (this.inputField != null)
            {
                this.inputField.asteriskChar = ch;
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.inputField = base.cachedComponent;
            }
            this.originalValue = this.inputField.asteriskChar;
            this.DoSetValue();
            base.Finish();
        }

        public override void OnExit()
        {
            if ((this.inputField != null) && this.resetOnExit.Value)
            {
                this.inputField.asteriskChar = this.originalValue;
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.asterixChar = "*";
            this.resetOnExit = null;
        }
    }
}

