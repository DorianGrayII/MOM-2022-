using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
    [Tooltip("Get the value of a Vector3 Variable from another FSM.")]
    public class GetFsmVector3 : FsmStateAction
    {
        [RequiredField]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.FsmName)]
        [Tooltip("Optional name of FSM on Game Object")]
        public FsmString fsmName;

        [RequiredField]
        [UIHint(UIHint.FsmVector3)]
        public FsmString variableName;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmVector3 storeValue;

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
            this.DoGetFsmVector3();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetFsmVector3();
        }

        private void DoGetFsmVector3()
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
                FsmVector3 fsmVector = this.fsm.FsmVariables.GetFsmVector3(this.variableName.Value);
                if (fsmVector != null)
                {
                    this.storeValue.Value = fsmVector.Value;
                }
            }
        }
    }
}
