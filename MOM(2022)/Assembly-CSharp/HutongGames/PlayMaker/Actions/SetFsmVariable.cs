namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.StateMachine), ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false), HutongGames.PlayMaker.Tooltip("Set the value of a variable in another FSM.")]
    public class SetFsmVariable : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.FsmName), HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
        public FsmString fsmName;
        [HutongGames.PlayMaker.Tooltip("The name of the variable in the target FSM.")]
        public FsmString variableName;
        [RequiredField]
        public FsmVar setValue;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;
        private PlayMakerFSM targetFsm;
        private NamedVariable targetVariable;
        private GameObject cachedGameObject;
        private string cachedFsmName;
        private string cachedVariableName;

        private void DoSetFsmVariable()
        {
            if (!this.setValue.IsNone && !string.IsNullOrEmpty(this.variableName.Value))
            {
                GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
                if (ownerDefaultTarget != null)
                {
                    if ((ownerDefaultTarget != this.cachedGameObject) || (this.fsmName.Value != this.cachedFsmName))
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
                    }
                    else
                    {
                        this.setValue.UpdateValue();
                        this.setValue.ApplyValueTo(this.targetVariable);
                    }
                }
            }
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

        public override void Reset()
        {
            this.gameObject = null;
            this.fsmName = "";
            this.setValue = new FsmVar();
        }
    }
}

