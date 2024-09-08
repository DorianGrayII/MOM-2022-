using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Sets the playback speed of the Animator. 1 is normal playback speed")]
    public class SetAnimatorSpeed : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The playBack speed")]
        public FsmFloat speed;

        [Tooltip("Repeat every frame. Useful for changing over time.")]
        public bool everyFrame;

        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = null;
            this.speed = null;
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
            this.DoPlaybackSpeed();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoPlaybackSpeed();
        }

        private void DoPlaybackSpeed()
        {
            if (!(this._animator == null))
            {
                this._animator.speed = this.speed.Value;
            }
        }
    }
}
