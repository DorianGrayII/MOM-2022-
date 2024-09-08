namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("Sets the Tinting Color for all background elements rendered by the GUI. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
    public class SetGUIBackgroundColor : FsmStateAction
    {
        [RequiredField]
        public FsmColor backgroundColor;
        public FsmBool applyGlobally;

        public override void OnGUI()
        {
            GUI.backgroundColor = this.backgroundColor.get_Value();
            if (this.applyGlobally.Value)
            {
                PlayMakerGUI.set_GUIBackgroundColor(GUI.backgroundColor);
            }
        }

        public override void Reset()
        {
            this.backgroundColor = (FsmColor) Color.white;
        }
    }
}

