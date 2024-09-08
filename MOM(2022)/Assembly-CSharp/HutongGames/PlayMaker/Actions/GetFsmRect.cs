using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
    [Tooltip("Get the value of a Rect Variable from another FSM.")]
    public class GetFsmRect : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject that owns the FSM.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.FsmName)]
        [Tooltip("Optional name of FSM on Game Object")]
        public FsmString fsmName;

        [RequiredField]
        [UIHint(UIHint.FsmRect)]
        public FsmString variableName;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmRect storeValue;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private GameObject goLastFrame;

        private string fsmNameLastFrame;

        protected PlayMakerFSM fsm;

        public override void Reset()
        {
            this.gameObject = null;
            this.fsmName = "";
            this.variableName = "";
            this.storeValue = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoGetFsmVariable();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetFsmVariable();
        }

        private void DoGetFsmVariable()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                return;
            }
            if (ownerDefaultTarget != this.goLastFrame || this.fsmName.Value != this.fsmNameLastFrame)
            {
                this.goLastFrame = ownerDefaultTarget;
                this.fsmNameLastFrame = this.fsmName.Value;
                this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
            }
            if (!(this.fsm == null) && this.storeValue != null)
            {
                FsmRect fsmRect = this.fsm.FsmVariables.GetFsmRect(this.variableName.Value);
                if (fsmRect != null)
                {
                    this.storeValue.Value = fsmRect.Value;
                }
            }
        }
    }
}
