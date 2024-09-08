namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Sets the playback position in the recording buffer. When in playback mode (use AnimatorStartPlayback), this value is used for controlling the current playback position in the buffer (in seconds). The value can range between recordingStartTime and recordingStopTime ")]
    public class SetAnimatorPlayBackTime : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The playBack time")]
        public FsmFloat playbackTime;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful for changing over time.")]
        public bool everyFrame;
        private Animator _animator;

        private void DoPlaybackTime()
        {
            if (this._animator != null)
            {
                this._animator.playbackTime = this.playbackTime.Value;
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
                    this.DoPlaybackTime();
                    if (!this.everyFrame)
                    {
                        base.Finish();
                    }
                }
            }
        }

        public override void OnUpdate()
        {
            this.DoPlaybackTime();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.playbackTime = null;
            this.everyFrame = false;
        }
    }
}

