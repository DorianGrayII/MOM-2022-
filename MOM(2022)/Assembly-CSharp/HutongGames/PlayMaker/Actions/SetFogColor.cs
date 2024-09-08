namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.RenderSettings), HutongGames.PlayMaker.Tooltip("Sets the color of the Fog in the scene.")]
    public class SetFogColor : FsmStateAction
    {
        [RequiredField]
        public FsmColor fogColor;
        public bool everyFrame;

        private void DoSetFogColor()
        {
            RenderSettings.fogColor = this.fogColor.get_Value();
        }

        public override void OnEnter()
        {
            this.DoSetFogColor();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetFogColor();
        }

        public override void Reset()
        {
            this.fogColor = (FsmColor) Color.white;
            this.everyFrame = false;
        }
    }
}

