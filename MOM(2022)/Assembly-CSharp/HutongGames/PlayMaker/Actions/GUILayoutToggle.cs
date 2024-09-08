namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("Makes an on/off Toggle Button and stores the button state in a Bool Variable.")]
    public class GUILayoutToggle : GUILayoutAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmBool storeButtonState;
        public FsmTexture image;
        public FsmString text;
        public FsmString tooltip;
        public FsmString style;
        public FsmEvent changedEvent;

        public override void OnGUI()
        {
            bool changed = GUI.changed;
            GUI.changed = false;
            this.storeButtonState.Value = GUILayout.Toggle(this.storeButtonState.Value, new GUIContent(this.text.Value, this.image.get_Value(), this.tooltip.Value), this.style.Value, base.LayoutOptions);
            if (!GUI.changed)
            {
                GUI.changed = changed;
            }
            else
            {
                base.Fsm.Event(this.changedEvent);
                GUIUtility.ExitGUI();
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.storeButtonState = null;
            this.text = "";
            this.image = null;
            this.tooltip = "";
            this.style = "Toggle";
            this.changedEvent = null;
        }
    }
}

