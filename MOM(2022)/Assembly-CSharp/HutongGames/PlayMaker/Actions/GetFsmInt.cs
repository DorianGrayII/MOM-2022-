namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.StateMachine), ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false), HutongGames.PlayMaker.Tooltip("Get the value of an Integer Variable from another FSM.")]
    public class GetFsmInt : FsmStateAction
    {
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.FsmName), HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
        public FsmString fsmName;
        [RequiredField, UIHint(UIHint.FsmInt)]
        public FsmString variableName;
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmInt storeValue;
        public bool everyFrame;
        private GameObject goLastFrame;
        private string fsmNameLastFrame;
        private PlayMakerFSM fsm;

        private void DoGetFsmInt()
        {
            if (this.storeValue != null)
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
                    if (this.fsm != null)
                    {
                        FsmInt fsmInt = this.fsm.FsmVariables.GetFsmInt(this.variableName.Value);
                        if (fsmInt != null)
                        {
                            this.storeValue.Value = fsmInt.Value;
                        }
                    }
                }
            }
        }

        public override void OnEnter()
        {
            this.DoGetFsmInt();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetFsmInt();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.fsmName = "";
            this.storeValue = null;
        }
    }
}

