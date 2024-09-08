namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("Begin a centered GUILayout block. The block is centered inside a parent GUILayout Area. So to place the block in the center of the screen, first use a GULayout Area the size of the whole screen (the default setting). NOTE: Block must end with a corresponding GUILayoutEndCentered.")]
    public class GUILayoutBeginCentered : FsmStateAction
    {
        public override void OnGUI()
        {
            GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
        }

        public override void Reset()
        {
        }
    }
}

