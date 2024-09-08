﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Gets the playback position in the recording buffer. When in playback mode (use  AnimatorStartPlayback), this value is used for controlling the current playback position in the buffer (in seconds). The value can range between recordingStartTime and recordingStopTime See Also: StartPlayback, StopPlayback.")]
    public class GetAnimatorPlayBackTime : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [ActionSection("Result"), RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The playBack time of the animator.")]
        public FsmFloat playBackTime;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
        public bool everyFrame;
        private Animator _animator;

        private void GetPlayBackTime()
        {
            if (this._animator != null)
            {
                this.playBackTime.Value = this._animator.playbackTime;
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
                    this.GetPlayBackTime();
                    if (!this.everyFrame)
                    {
                        base.Finish();
                    }
                }
            }
        }

        public override void OnUpdate()
        {
            this.GetPlayBackTime();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.playBackTime = null;
            this.everyFrame = false;
        }
    }
}

