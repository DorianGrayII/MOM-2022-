namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("Inserts a flexible space element.")]
    public class GUILayoutFlexibleSpace : FsmStateAction
    {
        public override void OnGUI()
        {
            GUILayout.FlexibleSpace();
        }

        public override void Reset()
        {
        }
    }
}

