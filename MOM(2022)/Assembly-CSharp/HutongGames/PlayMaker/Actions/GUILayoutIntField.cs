namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("GUILayout Text Field to edit an Int Variable. Optionally send an event if the text has been edited.")]
    public class GUILayoutIntField : GUILayoutAction
    {
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Int Variable to show in the edit field.")]
        public FsmInt intVariable;
        [HutongGames.PlayMaker.Tooltip("Optional GUIStyle in the active GUISKin.")]
        public FsmString style;
        [HutongGames.PlayMaker.Tooltip("Optional event to send when the value changes.")]
        public FsmEvent changedEvent;

        public override void OnGUI()
        {
            bool changed = GUI.changed;
            GUI.changed = false;
            this.intVariable.Value = string.IsNullOrEmpty(this.style.Value) ? int.Parse(GUILayout.TextField(this.intVariable.Value.ToString(), base.LayoutOptions)) : int.Parse(GUILayout.TextField(this.intVariable.Value.ToString(), this.style.Value, base.LayoutOptions));
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
            this.intVariable = null;
            this.style = "";
            this.changedEvent = null;
        }
    }
}

