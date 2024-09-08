namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Set Apply Root Motion: If true, Root is controlled by animations")]
    public class SetAnimatorApplyRootMotion : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("If true, Root is controlled by animations")]
        public FsmBool applyRootMotion;
        private Animator _animator;

        private void DoApplyRootMotion()
        {
            if (this._animator != null)
            {
                this._animator.applyRootMotion = this.applyRootMotion.Value;
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
                    this.DoApplyRootMotion();
                    base.Finish();
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.applyRootMotion = null;
        }
    }
}

