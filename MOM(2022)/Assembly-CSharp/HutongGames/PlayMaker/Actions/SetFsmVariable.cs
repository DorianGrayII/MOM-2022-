using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
    [Tooltip("Set the value of a variable in another FSM.")]
    public class SetFsmVariable : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject that owns the FSM")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.FsmName)]
        [Tooltip("Optional name of FSM on Game Object")]
        public FsmString fsmName;

        [Tooltip("The name of the variable in the target FSM.")]
        public FsmString variableName;

        [RequiredField]
        public FsmVar setValue;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private PlayMakerFSM targetFsm;

        private NamedVariable targetVariable;

        private GameObject cachedGameObject;

        private string cachedFsmName;

        private string cachedVariableName;

        public override void Reset()
        {
            this.gameObject = null;
            this.fsmName = "";
            this.setValue = new FsmVar();
        }

        public override void OnEnter()
        {
            this.DoSetFsmVariable();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetFsmVariable();
        }

        private void DoSetFsmVariable()
        {
            if (this.setValue.IsNone || string.IsNullOrEmpty(this.variableName.Value))
            {
                return;
            }
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                return;
            }
            if (ownerDefaultTarget != this.cachedGameObject || this.fsmName.Value != this.cachedFsmName)
            {
                this.targetFsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
                if (this.targetFsm == null)
                {
                    return;
                }
                this.cachedGameObject = ownerDefaultTarget;
                this.cachedFsmName = this.fsmName.Value;
            }
            if (this.variableName.Value != this.cachedVariableName)
            {
                this.targetVariable = this.targetFsm.FsmVariables.FindVariable(this.setValue.Type, this.variableName.Value);
                this.cachedVariableName = this.variableName.Value;
            }
            if (this.targetVariable == null)
            {
                base.LogWarning("Missing Variable: " + this.variableName.Value);
                return;
            }
            this.setValue.UpdateValue();
            this.setValue.ApplyValueTo(this.targetVariable);
        }
    }
}
