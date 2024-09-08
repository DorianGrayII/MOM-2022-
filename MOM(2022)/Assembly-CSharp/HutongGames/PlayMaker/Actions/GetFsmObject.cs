namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.StateMachine), ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false), HutongGames.PlayMaker.Tooltip("Get the value of an Object Variable from another FSM.")]
    public class GetFsmObject : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.FsmName), HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
        public FsmString fsmName;
        [RequiredField, UIHint(UIHint.FsmObject)]
        public FsmString variableName;
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmObject storeValue;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;
        private GameObject goLastFrame;
        private string fsmNameLastFrame;
        protected PlayMakerFSM fsm;

        private void DoGetFsmVariable()
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
                if ((this.fsm != null) && (this.storeValue != null))
                {
                    FsmObject fsmObject = this.fsm.FsmVariables.GetFsmObject(this.variableName.Value);
                    if (fsmObject != null)
                    {
                        this.storeValue.set_Value(fsmObject.get_Value());
                    }
                }
            }
        }

        public override void OnEnter()
        {
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

        public override void Reset()
        {
            this.gameObject = null;
            this.fsmName = "";
            this.variableName = "";
            this.storeValue = null;
            this.everyFrame = false;
        }
    }
}

