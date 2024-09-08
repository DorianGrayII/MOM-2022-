using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
    [Tooltip("Set the value of a Bool Variable in another FSM.")]
    public class SetFsmBool : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject that owns the FSM.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.FsmName)]
        [Tooltip("Optional name of FSM on Game Object")]
        public FsmString fsmName;

        [RequiredField]
        [UIHint(UIHint.FsmBool)]
        [Tooltip("The name of the FSM variable.")]
        public FsmString variableName;

        [RequiredField]
        [Tooltip("Set the value of the variable.")]
        public FsmBool setValue;

        [Tooltip("Repeat every frame. Useful if the value is changing.")]
        public bool everyFrame;

        private GameObject goLastFrame;

        private string fsmNameLastFrame;

        private PlayMakerFSM fsm;

        public override void Reset()
        {
            this.gameObject = null;
            this.fsmName = "";
            this.setValue = null;
        }

        public override void OnEnter()
        {
            this.DoSetFsmBool();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        private void DoSetFsmBool()
        {
            if (this.setValue == null)
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
            if (this.fsm == null)
            {
                base.LogWarning("Could not find FSM: " + this.fsmName.Value);
                return;
            }
            FsmBool fsmBool = this.fsm.FsmVariables.FindFsmBool(this.variableName.Value);
            if (fsmBool != null)
            {
                fsmBool.Value = this.setValue.Value;
            }
            else
            {
                base.LogWarning("Could not find variable: " + this.variableName.Value);
            }
        }

        public override void OnUpdate()
        {
            this.DoSetFsmBool();
        }
    }
}
