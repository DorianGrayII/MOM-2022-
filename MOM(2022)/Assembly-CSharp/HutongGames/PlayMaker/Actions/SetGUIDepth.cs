namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("Sets the sorting depth of subsequent GUI elements.")]
    public class SetGUIDepth : FsmStateAction
    {
        [RequiredField]
        public FsmInt depth;

        public override void OnGUI()
        {
            GUI.depth = this.depth.Value;
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleOnGUI = true;
        }

        public override void Reset()
        {
            this.depth = 0;
        }
    }
}

