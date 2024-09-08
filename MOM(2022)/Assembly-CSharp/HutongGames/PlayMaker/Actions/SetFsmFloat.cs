namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.StateMachine), ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false), HutongGames.PlayMaker.Tooltip("Set the value of a Float Variable in another FSM.")]
    public class SetFsmFloat : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.FsmName), HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
        public FsmString fsmName;
        [RequiredField, UIHint(UIHint.FsmFloat), HutongGames.PlayMaker.Tooltip("The name of the FSM variable.")]
        public FsmString variableName;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Set the value of the variable.")]
        public FsmFloat setValue;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is changing.")]
        public bool everyFrame;
        private GameObject goLastFrame;
        private string fsmNameLastFrame;
        private PlayMakerFSM fsm;

        private void DoSetFsmFloat()
        {
            if (this.setValue != null)
            {
                GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
                if (ownerDefaultTarget != null)
                {
                    if ((ownerDefaultTarget != this.goLastFrame) || (this.fsmName.Value != this.fsmNameLastFrame))
                    {
                        this.goLastFrame = ownerDefaultTarget;
                        this.fsmNameLastFrame = this.fsmName.Value;
                        this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
                    }
                    if (this.fsm == null)
                    {
                        base.LogWarning("Could not find FSM: " + this.fsmName.Value);
                    }
                    else
                    {
                        FsmFloat fsmFloat = this.fsm.FsmVariables.GetFsmFloat(this.variableName.Value);
                        if (fsmFloat != null)
                        {
                            fsmFloat.Value = this.setValue.Value;
                        }
                        else
                        {
                            base.LogWarning("Could not find variable: " + this.variableName.Value);
                        }
                    }
                }
            }
        }

        public override void OnEnter()
        {
            this.DoSetFsmFloat();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetFsmFloat();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.fsmName = "";
            this.setValue = null;
        }
    }
}

