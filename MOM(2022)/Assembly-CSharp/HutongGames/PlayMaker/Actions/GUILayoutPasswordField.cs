namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("GUILayout Password Field. Optionally send an event if the text has been edited.")]
    public class GUILayoutPasswordField : GUILayoutAction
    {
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The password Text")]
        public FsmString text;
        [HutongGames.PlayMaker.Tooltip("The Maximum Length of the field")]
        public FsmInt maxLength;
        [HutongGames.PlayMaker.Tooltip("The Style of the Field")]
        public FsmString style;
        [HutongGames.PlayMaker.Tooltip("Event sent when field content changed")]
        public FsmEvent changedEvent;
        [HutongGames.PlayMaker.Tooltip("Replacement character to hide the password")]
        public FsmString mask;

        public override void OnGUI()
        {
            bool changed = GUI.changed;
            GUI.changed = false;
            this.text.Value = GUILayout.PasswordField(this.text.Value, this.mask.Value[0], this.style.Value, base.LayoutOptions);
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
            this.text = null;
            this.maxLength = 0x19;
            this.style = "TextField";
            this.mask = "*";
            this.changedEvent = null;
        }
    }
}

