namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Audio), HutongGames.PlayMaker.Tooltip("Plays an Audio Clip at a position defined by a Game Object or Vector3. If a position is defined, it takes priority over the game object. This action doesn't require an Audio Source component, but offers less control than Audio actions.")]
    public class PlaySound : FsmStateAction
    {
        public FsmOwnerDefault gameObject;
        public FsmVector3 position;
        [RequiredField, Title("Audio Clip"), ObjectType(typeof(AudioClip))]
        public FsmObject clip;
        [HasFloatSlider(0f, 1f)]
        public FsmFloat volume = 1f;

        private void DoPlaySound()
        {
            AudioClip clip = this.clip.get_Value() as AudioClip;
            if (clip == null)
            {
                base.LogWarning("Missing Audio Clip!");
            }
            else if (!this.position.IsNone)
            {
                AudioSource.PlayClipAtPoint(clip, this.position.get_Value(), this.volume.Value);
            }
            else
            {
                GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
                if (ownerDefaultTarget != null)
                {
                    AudioSource.PlayClipAtPoint(clip, ownerDefaultTarget.transform.position, this.volume.Value);
                }
            }
        }

        public override void OnEnter()
        {
            this.DoPlaySound();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.position = vector1;
            this.clip = null;
            this.volume = 1f;
        }
    }
}

