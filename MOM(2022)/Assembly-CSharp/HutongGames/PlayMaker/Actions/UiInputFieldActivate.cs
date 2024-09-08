using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Activate a UI InputField component to begin processing Events. Optionally Deactivate on state exit")]
    public class UiInputFieldActivate : ComponentAction<InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(InputField))]
        [Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool deactivateOnExit;

        private InputField inputField;

        public override void Reset()
        {
            this.gameObject = null;
            this.deactivateOnExit = null;
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
                this.inputField.ActivateInputField();
            }
        }

        public override void OnExit()
        {
            if (!(this.inputField == null) && this.deactivateOnExit.Value)
            {
                this.inputField.DeactivateInputField();
            }
        }
    }
}
