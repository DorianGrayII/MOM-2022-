namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.StateMachine), ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false), HutongGames.PlayMaker.Tooltip("Enables/Disables an FSM component on a GameObject.")]
    public class EnableFSM : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM component.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.FsmName), HutongGames.PlayMaker.Tooltip("Optional name of FSM on GameObject. Useful if you have more than one FSM on a GameObject.")]
        public FsmString fsmName;
        [HutongGames.PlayMaker.Tooltip("Set to True to enable, False to disable.")]
        public FsmBool enable;
        [HutongGames.PlayMaker.Tooltip("Reset the initial enabled state when exiting the state.")]
        public FsmBool resetOnExit;
        private PlayMakerFSM fsmComponent;

        private void DoEnableFSM()
        {
            GameObject obj2 = (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.get_Owner() : this.gameObject.GameObject.get_Value();
            if (obj2 != null)
            {
                if (string.IsNullOrEmpty(this.fsmName.Value))
                {
                    this.fsmComponent = obj2.GetComponent<PlayMakerFSM>();
                }
                else
                {
                    foreach (PlayMakerFSM rfsm in obj2.GetComponents<PlayMakerFSM>())
                    {
                        if (rfsm.FsmName == this.fsmName.Value)
                        {
                            this.fsmComponent = rfsm;
                            break;
                        }
                    }
                }
                if (this.fsmComponent == null)
                {
                    base.LogError("Missing FsmComponent!");
                }
                else
                {
                    this.fsmComponent.enabled = this.enable.Value;
                }
            }
        }

        public override void OnEnter()
        {
            this.DoEnableFSM();
            base.Finish();
        }

        public override void OnExit()
        {
            if ((this.fsmComponent != null) && this.resetOnExit.Value)
            {
                this.fsmComponent.enabled = !this.enable.Value;
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.fsmName = "";
            this.enable = true;
            this.resetOnExit = true;
        }
    }
}

