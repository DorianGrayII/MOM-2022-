namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("Fills the screen with a Color. NOTE: Uses OnGUI so you need a PlayMakerGUI component in the scene.")]
    public class DrawFullscreenColor : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("Color. NOTE: Uses OnGUI so you need a PlayMakerGUI component in the scene.")]
        public FsmColor color;

        public override void OnGUI()
        {
            GUI.color = this.color.get_Value();
            GUI.DrawTexture(new Rect(0f, 0f, (float) Screen.width, (float) Screen.height), ActionHelpers.WhiteTexture);
            GUI.color = GUI.color;
        }

        public override void Reset()
        {
            this.color = (FsmColor) Color.white;
        }
    }
}

