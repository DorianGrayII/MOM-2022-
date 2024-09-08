namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Audio), HutongGames.PlayMaker.Tooltip("Sets the Audio Clip played by the AudioSource component on a Game Object.")]
    public class SetAudioClip : ComponentAction<AudioSource>
    {
        [RequiredField, CheckForComponent(typeof(AudioSource)), HutongGames.PlayMaker.Tooltip("The GameObject with the AudioSource component.")]
        public FsmOwnerDefault gameObject;
        [ObjectType(typeof(AudioClip)), HutongGames.PlayMaker.Tooltip("The AudioClip to set.")]
        public FsmObject audioClip;

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                base.audio.clip = this.audioClip.get_Value() as AudioClip;
            }
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.audioClip = null;
        }
    }
}

