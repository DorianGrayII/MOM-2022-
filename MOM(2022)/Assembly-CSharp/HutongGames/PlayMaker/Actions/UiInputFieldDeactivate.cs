namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Deactivate a UI InputField to stop the processing of Events and send OnSubmit if not canceled. Optionally Activate on state exit")]
    public class UiInputFieldDeactivate : ComponentAction<InputField>
    {
        [RequiredField, CheckForComponent(typeof(InputField)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Activate when exiting this state.")]
        public FsmBool activateOnExit;
        private InputField inputField;

        private void DoAction()
        {
            if (this.inputField != null)
            {
                this.inputField.DeactivateInputField();
            }
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

        public override void OnExit()
        {
            if ((this.inputField != null) && this.activateOnExit.Value)
            {
                this.inputField.ActivateInputField();
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.activateOnExit = null;
        }
    }
}

