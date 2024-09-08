namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("Close a GUILayout group started with BeginArea.")]
    public class GUILayoutEndArea : FsmStateAction
    {
        public override void OnGUI()
        {
            GUILayout.EndArea();
        }

        public override void Reset()
        {
        }
    }
}

