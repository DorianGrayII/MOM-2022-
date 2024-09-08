using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Deactivate a UI InputField to stop the processing of Events and send OnSubmit if not canceled. Optionally Activate on state exit")]
    public class UiInputFieldDeactivate : ComponentAction<InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(InputField))]
        [Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Activate when exiting this state.")]
        public FsmBool activateOnExit;

        private InputField inputField;

        public override void Reset()
        {
            this.gameObject = null;
            this.activateOnExit = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.inputField = base.cachedComponent;
            }
            this.DoAction();
            base.Finish();
        }

        private void DoAction()
        {
            if (this.inputField != null)
            {
                this.inputField.DeactivateInputField();
            }
        }

        public override void OnExit()
        {
            if (!(this.inputField == null) && this.activateOnExit.Value)
            {
                this.inputField.ActivateInputField();
            }
        }
    }
}
