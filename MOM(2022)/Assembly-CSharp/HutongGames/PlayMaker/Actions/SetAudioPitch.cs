namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Audio), HutongGames.PlayMaker.Tooltip("Sets the Pitch of the Audio Clip played by the AudioSource component on a Game Object.")]
    public class SetAudioPitch : ComponentAction<AudioSource>
    {
        [RequiredField, CheckForComponent(typeof(AudioSource))]
        public FsmOwnerDefault gameObject;
        public FsmFloat pitch;
        public bool everyFrame;

        private void DoSetAudioPitch()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget) && !this.pitch.IsNone)
            {
                base.audio.pitch = this.pitch.Value;
            }
        }

        public override void OnEnter()
        {
            this.DoSetAudioPitch();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetAudioPitch();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.pitch = 1f;
            this.everyFrame = false;
        }
    }
}

