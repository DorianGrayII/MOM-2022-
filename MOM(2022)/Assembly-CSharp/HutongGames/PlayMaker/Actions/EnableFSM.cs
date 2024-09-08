using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
    [Tooltip("Enables/Disables an FSM component on a GameObject.")]
    public class EnableFSM : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject that owns the FSM component.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.FsmName)]
        [Tooltip("Optional name of FSM on GameObject. Useful if you have more than one FSM on a GameObject.")]
        public FsmString fsmName;

        [Tooltip("Set to True to enable, False to disable.")]
        public FsmBool enable;

        [Tooltip("Reset the initial enabled state when exiting the state.")]
        public FsmBool resetOnExit;

        private PlayMakerFSM fsmComponent;

        public override void Reset()
        {
            this.gameObject = null;
            this.fsmName = "";
            this.enable = true;
            this.resetOnExit = true;
        }

        public override void OnEnter()
        {
            this.DoEnableFSM();
            base.Finish();
        }

        private void DoEnableFSM()
        {
            GameObject gameObject = ((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
            if (gameObject == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(this.fsmName.Value))
            {
                PlayMakerFSM[] components = gameObject.GetComponents<PlayMakerFSM>();
                foreach (PlayMakerFSM playMakerFSM in components)
                {
                    if (playMakerFSM.FsmName == this.fsmName.Value)
                    {
                        this.fsmComponent = playMakerFSM;
                        break;
                    }
                }
            }
            else
            {
                this.fsmComponent = gameObject.GetComponent<PlayMakerFSM>();
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

        public override void OnExit()
        {
            if (!(this.fsmComponent == null) && this.resetOnExit.Value)
            {
                this.fsmComponent.enabled = !this.enable.Value;
            }
        }
    }
}
