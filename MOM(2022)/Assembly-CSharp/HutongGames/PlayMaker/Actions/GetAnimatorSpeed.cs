using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Gets the playback speed of the Animator. 1 is normal playback speed")]
    public class GetAnimatorSpeed : FsmStateActionAnimatorBase
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The playBack speed of the animator. 1 is normal playback speed")]
        public FsmFloat speed;

        private Animator _animator;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.speed = null;
            base.everyFrame = false;
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
            this.GetPlaybackSpeed();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.GetPlaybackSpeed();
        }

        private void GetPlaybackSpeed()
        {
            if (this._animator != null)
            {
                this.speed.Value = this._animator.speed;
            }
        }
    }
}
