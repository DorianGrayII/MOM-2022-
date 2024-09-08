namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("Sets the Tinting Color for the GUI. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
    public class SetGUIColor : FsmStateAction
    {
        [RequiredField]
        public FsmColor color;
        public FsmBool applyGlobally;

        public override void OnGUI()
        {
            GUI.color = this.color.get_Value();
            if (this.applyGlobally.Value)
            {
                PlayMakerGUI.set_GUIColor(GUI.color);
            }
        }

        public override void Reset()
        {
            this.color = (FsmColor) Color.white;
        }
    }
}

