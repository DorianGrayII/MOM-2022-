namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.StateMachine), ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false), HutongGames.PlayMaker.Tooltip("Set the value of a Game Object Variable in another FSM. Accept null reference")]
    public class SetFsmGameObject : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.FsmName), HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
        public FsmString fsmName;
        [RequiredField, UIHint(UIHint.FsmGameObject), HutongGames.PlayMaker.Tooltip("The name of the FSM variable.")]
        public FsmString variableName;
        [HutongGames.PlayMaker.Tooltip("Set the value of the variable.")]
        public FsmGameObject setValue;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is changing.")]
        public bool everyFrame;
        private GameObject goLastFrame;
        private string fsmNameLastFrame;
        private PlayMakerFSM fsm;

        private void DoSetFsmGameObject()
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
                    FsmGameObject obj3 = this.fsm.FsmVariables.FindFsmGameObject(this.variableName.Value);
                    if (obj3 != null)
                    {
                        obj3.set_Value(this.setValue?.get_Value());
                    }
                    else
                    {
                        base.LogWarning("Could not find variable: " + this.variableName.Value);
                    }
                }
            }
        }

        public override void OnEnter()
        {
            this.DoSetFsmGameObject();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetFsmGameObject();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.fsmName = "";
            this.setValue = null;
            this.everyFrame = false;
        }
    }
}

