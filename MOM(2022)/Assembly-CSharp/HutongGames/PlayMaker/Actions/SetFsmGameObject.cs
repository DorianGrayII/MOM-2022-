using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
    [Tooltip("Set the value of a Game Object Variable in another FSM. Accept null reference")]
    public class SetFsmGameObject : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject that owns the FSM.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.FsmName)]
        [Tooltip("Optional name of FSM on Game Object")]
        public FsmString fsmName;

        [RequiredField]
        [UIHint(UIHint.FsmGameObject)]
        [Tooltip("The name of the FSM variable.")]
        public FsmString variableName;

        [Tooltip("Set the value of the variable.")]
        public FsmGameObject setValue;

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
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetFsmGameObject();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        private void DoSetFsmGameObject()
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
            if (!(this.fsm == null))
            {
                FsmGameObject fsmGameObject = this.fsm.FsmVariables.FindFsmGameObject(this.variableName.Value);
                if (fsmGameObject != null)
                {
                    fsmGameObject.Value = ((this.setValue == null) ? null : this.setValue.Value);
                }
                else
                {
                    base.LogWarning("Could not find variable: " + this.variableName.Value);
                }
            }
        }

        public override void OnUpdate()
        {
            this.DoSetFsmGameObject();
        }
    }
}
