using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Audio)]
    [Tooltip("Plays a Random Audio Clip at a position defined by a Game Object or a Vector3. If a position is defined, it takes priority over the game object. You can set the relative weight of the clips to control how often they are selected.")]
    public class PlayRandomSound : FsmStateAction
    {
        [Tooltip("The GameObject to play the sound.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Use world position instead of GameObject.")]
        public FsmVector3 position;

        [CompoundArray("Audio Clips", "Audio Clip", "Weight")]
        [ObjectType(typeof(AudioClip))]
        public FsmObject[] audioClips;

        [HasFloatSlider(0f, 1f)]
        public FsmFloat[] weights;

        [HasFloatSlider(0f, 1f)]
        public FsmFloat volume = 1f;

        [Tooltip("Don't play the same sound twice in a row")]
        public FsmBool noRepeat;

        private int randomIndex;

        private int lastIndex = -1;

        public override void Reset()
        {
            this.gameObject = null;
            this.position = new FsmVector3
            {
                UseVariable = true
            };
            this.audioClips = new FsmObject[3];
            this.weights = new FsmFloat[3] { 1f, 1f, 1f };
            this.volume = 1f;
            this.noRepeat = false;
        }

        public override void OnEnter()
        {
            this.DoPlayRandomClip();
            base.Finish();
        }

        private void DoPlayRandomClip()
        {
            if (this.audioClips.Length == 0)
            {
                return;
            }
            if (!this.noRepeat.Value || this.weights.Length == 1)
            {
                this.randomIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
            }
            else
            {
                do
                {
                    this.randomIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
                }
                while (this.randomIndex == this.lastIndex && this.randomIndex != -1);
                this.lastIndex = this.randomIndex;
            }
            if (this.randomIndex == -1)
            {
                return;
            }
            AudioClip audioClip = this.audioClips[this.randomIndex].Value as AudioClip;
            if (!(audioClip != null))
            {
                return;
            }
            if (!this.position.IsNone)
            {
                AudioSource.PlayClipAtPoint(audioClip, this.position.Value, this.volume.Value);
                return;
            }
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!(ownerDefaultTarget == null))
            {
                AudioSource.PlayClipAtPoint(audioClip, ownerDefaultTarget.transform.position, this.volume.Value);
            }
        }
    }
}
