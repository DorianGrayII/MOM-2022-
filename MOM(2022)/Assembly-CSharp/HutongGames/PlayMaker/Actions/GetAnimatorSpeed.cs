namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Gets the playback speed of the Animator. 1 is normal playback speed")]
    public class GetAnimatorSpeed : FsmStateActionAnimatorBase
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The playBack speed of the animator. 1 is normal playback speed")]
        public FsmFloat speed;
        private Animator _animator;

        private void GetPlaybackSpeed()
        {
            if (this._animator != null)
            {
                this.speed.Value = this._animator.speed;
            }
        }

        public override void OnActionUpdate()
        {
            this.GetPlaybackSpeed();
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
                    this.GetPlaybackSpeed();
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
            this.speed = null;
            base.everyFrame = false;
        }
    }
}

