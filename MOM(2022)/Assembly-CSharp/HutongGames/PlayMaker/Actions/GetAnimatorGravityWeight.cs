namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Returns The current gravity weight based on current animations that are played")]
    public class GetAnimatorGravityWeight : FsmStateActionAnimatorBase
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [ActionSection("Results"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The current gravity weight based on current animations that are played")]
        public FsmFloat gravityWeight;
        private Animator _animator;

        private void DoGetGravityWeight()
        {
            if (this._animator != null)
            {
                this.gravityWeight.Value = this._animator.gravityWeight;
            }
        }

        public override void OnActionUpdate()
        {
            this.DoGetGravityWeight();
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
                    this.DoGetGravityWeight();
                    if (!base.everyFrame)
                    {
                        base.Finish();
                    }
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.gravityWeight = null;
            base.everyFrame = false;
        }
    }
}

