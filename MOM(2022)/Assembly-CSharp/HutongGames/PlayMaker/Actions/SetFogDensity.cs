namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.RenderSettings), HutongGames.PlayMaker.Tooltip("Sets the density of the Fog in the scene.")]
    public class SetFogDensity : FsmStateAction
    {
        [RequiredField]
        public FsmFloat fogDensity;
        public bool everyFrame;

        private void DoSetFogDensity()
        {
            RenderSettings.fogDensity = this.fogDensity.Value;
        }

        public override void OnEnter()
        {
            this.DoSetFogDensity();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetFogDensity();
        }

        public override void Reset()
        {
            this.fogDensity = 0.5f;
            this.everyFrame = false;
        }
    }
}

