namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Audio), HutongGames.PlayMaker.Tooltip("Mute/unmute the Audio Clip played by an Audio Source component on a Game Object.")]
    public class AudioMute : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(AudioSource)), HutongGames.PlayMaker.Tooltip("The GameObject with an Audio Source component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Check to mute, uncheck to unmute.")]
        public FsmBool mute;

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
                if (component != null)
                {
                    component.mute = this.mute.Value;
                }
            }
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.mute = false;
        }
    }
}

