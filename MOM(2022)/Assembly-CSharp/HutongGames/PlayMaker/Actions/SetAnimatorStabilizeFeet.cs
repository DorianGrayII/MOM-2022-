namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("If true, automatically stabilize feet during transition and blending")]
    public class SetAnimatorStabilizeFeet : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("If true, automatically stabilize feet during transition and blending")]
        public FsmBool stabilizeFeet;
        private Animator _animator;

        private void DoStabilizeFeet()
        {
            if (this._animator != null)
            {
                this._animator.stabilizeFeet = this.stabilizeFeet.Value;
            }
        }

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
                    this.DoStabilizeFeet();
                    base.Finish();
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.stabilizeFeet = null;
        }
    }
}

