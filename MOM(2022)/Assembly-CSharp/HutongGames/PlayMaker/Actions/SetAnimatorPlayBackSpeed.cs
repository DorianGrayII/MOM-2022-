namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Sets the playback speed of the Animator. 1 is normal playback speed")]
    public class SetAnimatorPlayBackSpeed : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("If true, automatically stabilize feet during transition and blending")]
        public FsmFloat playBackSpeed;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful for changing over time.")]
        public bool everyFrame;
        private Animator _animator;

        private void DoPlayBackSpeed()
        {
            if (this._animator != null)
            {
                this._animator.speed = this.playBackSpeed.Value;
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
                    this.DoPlayBackSpeed();
                    if (!this.everyFrame)
                    {
                        base.Finish();
                    }
                }
            }
        }

        public override void OnUpdate()
        {
            this.DoPlayBackSpeed();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.playBackSpeed = null;
            this.everyFrame = false;
        }
    }
}

