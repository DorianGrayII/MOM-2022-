namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Audio), HutongGames.PlayMaker.Tooltip("Plays a Random Audio Clip at a position defined by a Game Object or a Vector3. If a position is defined, it takes priority over the game object. You can set the relative weight of the clips to control how often they are selected.")]
    public class PlayRandomSound : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject to play the sound.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Use world position instead of GameObject.")]
        public FsmVector3 position;
        [CompoundArray("Audio Clips", "Audio Clip", "Weight"), ObjectType(typeof(AudioClip))]
        public FsmObject[] audioClips;
        [HasFloatSlider(0f, 1f)]
        public FsmFloat[] weights;
        [HasFloatSlider(0f, 1f)]
        public FsmFloat volume = 1f;
        [HutongGames.PlayMaker.Tooltip("Don't play the same sound twice in a row")]
        public FsmBool noRepeat;
        private int randomIndex;
        private int lastIndex = -1;

        private void DoPlayRandomClip()
        {
            if (this.audioClips.Length != 0)
            {
                if (!this.noRepeat.Value || (this.weights.Length == 1))
                {
                    this.randomIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
                }
                else
                {
                    while (true)
                    {
                        this.randomIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
                        if ((this.randomIndex != this.lastIndex) || (this.randomIndex == -1))
                        {
                            this.lastIndex = this.randomIndex;
                            break;
                        }
                    }
                }
                if (this.randomIndex != -1)
                {
                    AudioClip clip = this.audioClips[this.randomIndex].get_Value() as AudioClip;
                    if (clip != null)
                    {
                        if (!this.position.IsNone)
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
                }
            }
        }

        public override void OnEnter()
        {
            this.DoPlayRandomClip();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.position = vector1;
            this.audioClips = new FsmObject[3];
            this.weights = new FsmFloat[] { 1f, 1f, 1f };
            this.volume = 1f;
            this.noRepeat = false;
        }
    }
}

