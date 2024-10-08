using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
    [Tooltip("Get the value of a variable in another FSM and store it in a variable of the same name in this FSM.")]
    public class GetFsmVariable : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject that owns the FSM")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.FsmName)]
        [Tooltip("Optional name of FSM on Game Object")]
        public FsmString fsmName;

        [RequiredField]
        [HideTypeFilter]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the value of the FsmVariable")]
        public FsmVar storeValue;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private GameObject cachedGO;

        private string cachedFsmName;

        private PlayMakerFSM sourceFsm;

        private INamedVariable sourceVariable;

        private NamedVariable targetVariable;

        public override void Reset()
        {
            this.gameObject = null;
            this.fsmName = "";
            this.storeValue = new FsmVar();
        }

        public override void OnEnter()
        {
            this.InitFsmVar();
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

        private void InitFsmVar()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!(ownerDefaultTarget == null) && (ownerDefaultTarget != this.cachedGO || this.cachedFsmName != this.fsmName.Value))
            {
                this.sourceFsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
                this.sourceVariable = this.sourceFsm.FsmVariables.GetVariable(this.storeValue.variableName);
                this.targetVariable = base.Fsm.Variables.GetVariable(this.storeValue.variableName);
                this.storeValue.Type = this.targetVariable.VariableType;
                if (!string.IsNullOrEmpty(this.storeValue.variableName) && this.sourceVariable == null)
                {
                    base.LogWarning("Missing Variable: " + this.storeValue.variableName);
                }
                this.cachedGO = ownerDefaultTarget;
                this.cachedFsmName = this.fsmName.Value;
            }
        }

        private void DoGetFsmVariable()
        {
            if (!this.storeValue.IsNone)
            {
                this.InitFsmVar();
                this.storeValue.GetValueFrom(this.sourceVariable);
                this.storeValue.ApplyValueTo(this.targetVariable);
            }
        }
    }
}
