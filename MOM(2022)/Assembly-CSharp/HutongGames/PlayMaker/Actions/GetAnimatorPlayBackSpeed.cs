using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Gets the playback speed of the Animator. 1 is normal playback speed")]
    public class GetAnimatorPlayBackSpeed : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The playBack speed of the animator. 1 is normal playback speed")]
        public FsmFloat playBackSpeed;

        [Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
        public bool everyFrame;

        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = null;
            this.playBackSpeed = null;
            this.everyFrame = false;
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
            this.GetPlayBackSpeed();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.GetPlayBackSpeed();
        }

        private void GetPlayBackSpeed()
        {
            if (this._animator != null)
            {
                this.playBackSpeed.Value = this._animator.speed;
            }
        }
    }
}
