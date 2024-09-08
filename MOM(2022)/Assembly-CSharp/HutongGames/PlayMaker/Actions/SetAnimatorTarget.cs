namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Sets an AvatarTarget and a targetNormalizedTime for the current state")]
    public class SetAnimatorTarget : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The avatar target")]
        public AvatarTarget avatarTarget;
        [HutongGames.PlayMaker.Tooltip("The current state Time that is queried")]
        public FsmFloat targetNormalizedTime;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame during OnAnimatorMove. Useful when changing over time.")]
        public bool everyFrame;
        private Animator _animator;

        public override void DoAnimatorMove()
        {
            this.SetTarget();
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
                    this.SetTarget();
                    if (!this.everyFrame)
                    {
                        base.Finish();
                    }
                }
            }
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleAnimatorMove = true;
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.avatarTarget = AvatarTarget.Body;
            this.targetNormalizedTime = null;
            this.everyFrame = false;
        }

        private void SetTarget()
        {
            if (this._animator != null)
            {
                this._animator.SetTarget(this.avatarTarget, this.targetNormalizedTime.Value);
            }
        }
    }
}

