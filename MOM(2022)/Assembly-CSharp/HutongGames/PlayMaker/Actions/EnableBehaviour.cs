namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.ScriptControl), HutongGames.PlayMaker.Tooltip("Enables/Disables a Behaviour on a GameObject. Optionally reset the Behaviour on exit - useful if you want the Behaviour to be active only while this state is active.")]
    public class EnableBehaviour : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the Behaviour.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Behaviour), HutongGames.PlayMaker.Tooltip("The name of the Behaviour to enable/disable.")]
        public FsmString behaviour;
        [HutongGames.PlayMaker.Tooltip("Optionally drag a component directly into this field (behavior name will be ignored).")]
        public Component component;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Set to True to enable, False to disable.")]
        public FsmBool enable;
        public FsmBool resetOnExit;
        private Behaviour componentTarget;

        private void DoEnableBehaviour(GameObject go)
        {
            if (go != null)
            {
                this.componentTarget = (this.component == null) ? (go.GetComponent(ReflectionUtils.GetGlobalType(this.behaviour.Value)) as Behaviour) : (this.component as Behaviour);
                if (this.componentTarget == null)
                {
                    base.LogWarning(" " + go.name + " missing behaviour: " + this.behaviour.Value);
                }
                else
                {
                    this.componentTarget.enabled = this.enable.Value;
                }
            }
        }

        public override string ErrorCheck()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            return (((ownerDefaultTarget == null) || ((this.component != null) || (this.behaviour.IsNone || string.IsNullOrEmpty(this.behaviour.Value)))) ? null : ((ownerDefaultTarget.GetComponent(ReflectionUtils.GetGlobalType(this.behaviour.Value)) is Behaviour) ? null : "Behaviour missing"));
        }

        public override void OnEnter()
        {
            this.DoEnableBehaviour(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
            base.Finish();
        }

        public override void OnExit()
        {
            if ((this.componentTarget != null) && this.resetOnExit.Value)
            {
                this.componentTarget.enabled = !this.enable.Value;
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.behaviour = null;
            this.component = null;
            this.enable = true;
            this.resetOnExit = true;
        }
    }
}

