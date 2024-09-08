using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
    [Tooltip("Set the value of a String Variable in another FSM.")]
    public class SetFsmEnum : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject that owns the FSM.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.FsmName)]
        [Tooltip("Optional name of FSM on Game Object.")]
        public FsmString fsmName;

        [RequiredField]
        [UIHint(UIHint.FsmEnum)]
        [Tooltip("Enum variable name needs to match the FSM variable name on Game Object.")]
        public FsmString variableName;

        [RequiredField]
        public FsmEnum setValue;

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
            this.DoSetFsmEnum();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        private void DoSetFsmEnum()
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
            FsmEnum fsmEnum = this.fsm.FsmVariables.GetFsmEnum(this.variableName.Value);
            if (fsmEnum != null)
            {
                fsmEnum.Value = this.setValue.Value;
            }
            else
            {
                base.LogWarning("Could not find variable: " + this.variableName.Value);
            }
        }

        public override void OnUpdate()
        {
            this.DoSetFsmEnum();
        }
    }
}
