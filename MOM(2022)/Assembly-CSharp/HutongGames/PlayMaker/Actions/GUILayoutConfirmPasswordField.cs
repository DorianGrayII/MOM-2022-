using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GUILayout)]
    [Tooltip("GUILayout Password Field. Optionally send an event if the text has been edited.")]
    public class GUILayoutConfirmPasswordField : GUILayoutAction
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("The password Text")]
        public FsmString text;

        [Tooltip("The Maximum Length of the field")]
        public FsmInt maxLength;

        [Tooltip("The Style of the Field")]
        public FsmString style;

        [Tooltip("Event sent when field content changed")]
        public FsmEvent changedEvent;

        [Tooltip("Replacement character to hide the password")]
        public FsmString mask;

        [Tooltip("GUILayout Password Field. Optionally send an event if the text has been edited.")]
        public FsmBool confirm;

        [Tooltip("Confirmation content")]
        public FsmString password;

        public override void Reset()
        {
            this.text = null;
            this.maxLength = 25;
            this.style = "TextField";
            this.mask = "*";
            this.changedEvent = null;
            this.confirm = false;
            this.password = null;
        }

        public override void OnGUI()
        {
            bool changed = GUI.changed;
            GUI.changed = false;
            this.text.Value = GUILayout.PasswordField(this.text.Value, this.mask.Value[0], this.style.Value, base.LayoutOptions);
            if (GUI.changed)
            {
                base.Fsm.Event(this.changedEvent);
                GUIUtility.ExitGUI();
            }
            else
            {
                GUI.changed = changed;
            }
        }
    }
}
