namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Sets the animator in recording mode, and allocates a circular buffer of size frameCount. After this call, the recorder starts collecting up to frameCount frames in the buffer. Note it is not possible to start playback until a call to StopRecording is made")]
    public class AnimatorStartRecording : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The number of frames (updates) that will be recorded. If frameCount is 0, the recording will continue until the user calls StopRecording. The maximum value for frameCount is 10000.")]
        public FsmInt frameCount;

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                base.Finish();
            }
            else
            {
                Animator component = ownerDefaultTarget.GetComponent<Animator>();
                if (component != null)
                {
                    component.StartRecording(this.frameCount.Value);
                }
                base.Finish();
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.frameCount = 0;
        }
    }
}

