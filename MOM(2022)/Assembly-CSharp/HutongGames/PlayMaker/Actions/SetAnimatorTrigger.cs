namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Sets a trigger parameter to active. Triggers are parameters that act mostly like booleans, but get reset to inactive when they are used in a transition.")]
    public class SetAnimatorTrigger : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.AnimatorTrigger), HutongGames.PlayMaker.Tooltip("The trigger name")]
        public FsmString trigger;
        private Animator _animator;

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                base.Finish();
            }
            else
            {
                this._animator = ownerDefaultTarget.GetComponent<Animator>();
                if (this._animator == null)
                {
                    base.Finish();
                }
                else
                {
                    this.SetTrigger();
                    base.Finish();
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.trigger = null;
        }

        private void SetTrigger()
        {
            if (this._animator != null)
            {
                this._animator.SetTrigger(this.trigger.Value);
            }
        }
    }
}

