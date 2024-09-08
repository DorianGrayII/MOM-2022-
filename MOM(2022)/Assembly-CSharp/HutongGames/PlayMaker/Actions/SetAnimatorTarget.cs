using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Sets an AvatarTarget and a targetNormalizedTime for the current state")]
    public class SetAnimatorTarget : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The avatar target")]
        public AvatarTarget avatarTarget;

        [Tooltip("The current state Time that is queried")]
        public FsmFloat targetNormalizedTime;

        [Tooltip("Repeat every frame during OnAnimatorMove. Useful when changing over time.")]
        public bool everyFrame;

        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = null;
            this.avatarTarget = AvatarTarget.Body;
            this.targetNormalizedTime = null;
            this.everyFrame = false;
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleAnimatorMove = true;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                base.Finish();
                return;
            }
            this._animator = ownerDefaultTarget.GetComponent<Animator>();
            if (this._animator == null)
            {
                base.Finish();
                return;
            }
            this.SetTarget();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void DoAnimatorMove()
        {
            this.SetTarget();
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
