using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the caret's blink rate of a UI InputField component.")]
    public class UiInputFieldSetCaretBlinkRate : ComponentAction<InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(InputField))]
        [Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The caret's blink rate for the UI InputField component.")]
        public FsmInt caretBlinkRate;

        [Tooltip("Deactivate when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private InputField inputField;

        private float originalValue;

        public override void Reset()
        {
            this.gameObject = null;
            this.caretBlinkRate = null;
            this.resetOnExit = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.inputField = base.cachedComponent;
            }
            this.originalValue = this.inputField.caretBlinkRate;
            this.DoSetValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetValue();
        }

        private void DoSetValue()
        {
            if (this.inputField != null)
            {
                this.inputField.caretBlinkRate = this.caretBlinkRate.Value;
            }
        }

        public override void OnExit()
        {
            if (!(this.inputField == null) && this.resetOnExit.Value)
            {
                this.inputField.caretBlinkRate = this.originalValue;
            }
        }
    }
}
