namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Activate a UI InputField component to begin processing Events. Optionally Deactivate on state exit")]
    public class UiInputFieldActivate : ComponentAction<InputField>
    {
        [RequiredField, CheckForComponent(typeof(InputField)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool deactivateOnExit;
        private InputField inputField;

        private void DoAction()
        {
            if (this.inputField != null)
            {
                this.inputField.ActivateInputField();
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
            if ((this.inputField != null) && this.deactivateOnExit.Value)
            {
                this.inputField.DeactivateInputField();
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.deactivateOnExit = null;
        }
    }
}

