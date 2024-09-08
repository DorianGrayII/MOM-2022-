namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Audio), ActionTarget(typeof(AudioSource), "gameObject", false), ActionTarget(typeof(AudioClip), "oneShotClip", false), HutongGames.PlayMaker.Tooltip("Plays the Audio Clip set with Set Audio Clip or in the Audio Source inspector on a Game Object. Optionally plays a one shot Audio Clip.")]
    public class AudioPlay : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(AudioSource)), HutongGames.PlayMaker.Tooltip("The GameObject with an AudioSource component.")]
        public FsmOwnerDefault gameObject;
        [HasFloatSlider(0f, 1f), HutongGames.PlayMaker.Tooltip("Set the volume.")]
        public FsmFloat volume;
        [ObjectType(typeof(AudioClip)), HutongGames.PlayMaker.Tooltip("Optionally play a 'one shot' AudioClip. NOTE: Volume cannot be adjusted while playing a 'one shot' AudioClip.")]
        public FsmObject oneShotClip;
        [HutongGames.PlayMaker.Tooltip("Wait until the end of the clip to send the Finish Event. Set to false to send the finish event immediately.")]
        public FsmBool WaitForEndOfClip;
        [HutongGames.PlayMaker.Tooltip("Event to send when the action finishes.")]
        public FsmEvent finishedEvent;
        private AudioSource audio;

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this.audio = ownerDefaultTarget.GetComponent<AudioSource>();
                if (this.audio != null)
                {
                    AudioClip clip = this.oneShotClip.get_Value() as AudioClip;
                    if (clip == null)
                    {
                        this.audio.Play();
                        if (!this.volume.IsNone)
                        {
                            this.audio.volume = this.volume.Value;
                        }
                        return;
                    }
                    if (!this.volume.IsNone)
                    {
                        this.audio.PlayOneShot(clip, this.volume.Value);
                    }
                    else
                    {
                        this.audio.PlayOneShot(clip);
                    }
                    if (!this.WaitForEndOfClip.Value)
                    {
                        base.Fsm.Event(this.finishedEvent);
                        base.Finish();
                    }
                    return;
                }
            }
            base.Finish();
        }

        public override void OnUpdate()
        {
            if (this.audio == null)
            {
                base.Finish();
            }
            else if (!this.audio.isPlaying)
            {
                base.Fsm.Event(this.finishedEvent);
                base.Finish();
            }
            else if (!this.volume.IsNone && (this.volume.Value != this.audio.volume))
            {
                this.audio.volume = this.volume.Value;
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.volume = 1f;
            this.oneShotClip = null;
            this.finishedEvent = null;
            this.WaitForEndOfClip = true;
        }
    }
}

