namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.StateMachine), ActionTarget(typeof(PlayMakerFSM), "fsmComponent", false), HutongGames.PlayMaker.Tooltip("Gets the name of the specified FSMs current state. Either reference the fsm component directly, or find it on a game object.")]
    public class GetFsmState : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Drag a PlayMakerFSM component here.")]
        public PlayMakerFSM fsmComponent;
        [HutongGames.PlayMaker.Tooltip("If not specifying the component above, specify the GameObject that owns the FSM")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.FsmName), HutongGames.PlayMaker.Tooltip("Optional name of Fsm on Game Object. If left blank it will find the first PlayMakerFSM on the GameObject.")]
        public FsmString fsmName;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the state name in a string variable.")]
        public FsmString storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. E.g.,  useful if you're waiting for the state to change.")]
        public bool everyFrame;
        private PlayMakerFSM fsm;

        private void DoGetFsmState()
        {
            if (this.fsm == null)
            {
                if (this.fsmComponent != null)
                {
                    this.fsm = this.fsmComponent;
                }
                else
                {
                    GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
                    if (ownerDefaultTarget != null)
                    {
                        this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
                    }
                }
                if (this.fsm == null)
                {
                    this.storeResult.Value = "";
                    return;
                }
            }
            this.storeResult.Value = this.fsm.ActiveStateName;
        }

        public override void OnEnter()
        {
            this.DoGetFsmState();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetFsmState();
        }

        public override void Reset()
        {
            this.fsmComponent = null;
            this.gameObject = null;
            this.fsmName = "";
            this.storeResult = null;
            this.everyFrame = false;
        }
    }
}

