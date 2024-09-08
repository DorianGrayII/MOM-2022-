namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("Resets the GUI matrix. Useful if you've rotated or scaled the GUI and now want to reset it.")]
    public class ResetGUIMatrix : FsmStateAction
    {
        public override void OnGUI()
        {
            Matrix4x4 identity = Matrix4x4.identity;
            GUI.matrix = identity;
            PlayMakerGUI.set_GUIMatrix(identity);
        }
    }
}

