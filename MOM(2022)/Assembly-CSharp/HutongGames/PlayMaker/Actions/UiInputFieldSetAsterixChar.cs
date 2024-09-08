using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the Asterix Character of a UI InputField component.")]
    public class UiInputFieldSetAsterixChar : ComponentAction<InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(InputField))]
        [Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The asterix Character used for password field type of the UI InputField component. Only the first character will be used, the rest of the string will be ignored")]
        public FsmString asterixChar;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        private InputField inputField;

        private char originalValue;

        private static char __char__ = ' ';

        public override void Reset()
        {
            this.gameObject = null;
            this.asterixChar = "*";
            this.resetOnExit = null;
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

        private void DoSetValue()
        {
            char asteriskChar = UiInputFieldSetAsterixChar.__char__;
            if (this.asterixChar.Value.Length > 0)
            {
                asteriskChar = this.asterixChar.Value[0];
            }
            if (this.inputField != null)
            {
                this.inputField.asteriskChar = asteriskChar;
            }
        }

        public override void OnExit()
        {
            if (!(this.inputField == null) && this.resetOnExit.Value)
            {
                this.inputField.asteriskChar = this.originalValue;
            }
        }
    }
}
