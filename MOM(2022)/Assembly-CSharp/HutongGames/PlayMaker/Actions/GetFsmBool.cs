using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
    [Tooltip("Get the value of a Bool Variable from another FSM.")]
    public class GetFsmBool : FsmStateAction
    {
        [RequiredField]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.FsmName)]
        [Tooltip("Optional name of FSM on Game Object")]
        public FsmString fsmName;

        [RequiredField]
        [UIHint(UIHint.FsmBool)]
        public FsmString variableName;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmBool storeValue;

        public bool everyFrame;

        private GameObject goLastFrame;

        private string fsmNameLastFrame;

        private PlayMakerFSM fsm;

        public override void Reset()
        {
            this.gameObject = null;
            this.fsmName = "";
            this.storeValue = null;
        }

        public override void OnEnter()
        {
            this.DoGetFsmBool();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetFsmBool();
        }

        private void DoGetFsmBool()
        {
            if (this.storeValue == null)
            {
                return;
            }
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
            if (!(this.fsm == null))
            {
                FsmBool fsmBool = this.fsm.FsmVariables.GetFsmBool(this.variableName.Value);
                if (fsmBool != null)
                {
                    this.storeValue.Value = fsmBool.Value;
                }
            }
        }
    }
}
