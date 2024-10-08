using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
    [Tooltip("Get the values of multiple variables in another FSM and store in variables of the same name in this FSM.")]
    public class GetFsmVariables : FsmStateAction
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
        [Tooltip("Store the values of the FsmVariables")]
        public FsmVar[] getVariables;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private GameObject cachedGO;

        private string cachedFsmName;

        private PlayMakerFSM sourceFsm;

        private INamedVariable[] sourceVariables;

        private NamedVariable[] targetVariables;

        public override void Reset()
        {
            this.gameObject = null;
            this.fsmName = "";
            this.getVariables = null;
        }

        private void InitFsmVars()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null || (!(ownerDefaultTarget != this.cachedGO) && !(this.cachedFsmName != this.fsmName.Value)))
            {
                return;
            }
            this.sourceVariables = new INamedVariable[this.getVariables.Length];
            this.targetVariables = new NamedVariable[this.getVariables.Length];
            for (int i = 0; i < this.getVariables.Length; i++)
            {
                string variableName = this.getVariables[i].variableName;
                this.sourceFsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
                this.sourceVariables[i] = this.sourceFsm.FsmVariables.GetVariable(variableName);
                this.targetVariables[i] = base.Fsm.Variables.GetVariable(variableName);
                this.getVariables[i].Type = this.targetVariables[i].VariableType;
                if (!string.IsNullOrEmpty(variableName) && this.sourceVariables[i] == null)
                {
                    base.LogWarning("Missing Variable: " + variableName);
                }
                this.cachedGO = ownerDefaultTarget;
                this.cachedFsmName = this.fsmName.Value;
            }
        }

        public override void OnEnter()
        {
            this.InitFsmVars();
            this.DoGetFsmVariables();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetFsmVariables();
        }

        private void DoGetFsmVariables()
        {
            this.InitFsmVars();
            for (int i = 0; i < this.getVariables.Length; i++)
            {
                this.getVariables[i].GetValueFrom(this.sourceVariables[i]);
                this.getVariables[i].ApplyValueTo(this.targetVariables[i]);
            }
        }
    }
}
