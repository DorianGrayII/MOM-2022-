namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Audio), HutongGames.PlayMaker.Tooltip("Sets the global sound volume.")]
    public class SetGameVolume : FsmStateAction
    {
        [RequiredField, HasFloatSlider(0f, 1f)]
        public FsmFloat volume;
        public bool everyFrame;

        public override void OnEnter()
        {
            AudioListener.volume = this.volume.Value;
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            AudioListener.volume = this.volume.Value;
        }

        public override void Reset()
        {
            this.volume = 1f;
            this.everyFrame = false;
        }
    }
}

