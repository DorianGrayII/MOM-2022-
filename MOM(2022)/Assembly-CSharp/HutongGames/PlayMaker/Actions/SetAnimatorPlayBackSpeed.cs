using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Sets the playback speed of the Animator. 1 is normal playback speed")]
    public class SetAnimatorPlayBackSpeed : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [Tooltip("If true, automatically stabilize feet during transition and blending")]
        public FsmFloat playBackSpeed;

        [Tooltip("Repeat every frame. Useful for changing over time.")]
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
            this.DoPlayBackSpeed();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoPlayBackSpeed();
        }

        private void DoPlayBackSpeed()
        {
            if (!(this._animator == null))
            {
                this._animator.speed = this.playBackSpeed.Value;
            }
        }
    }
}
