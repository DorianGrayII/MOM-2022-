namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("GUILayout Text Field to edit a Float Variable. Optionally send an event if the text has been edited.")]
    public class GUILayoutFloatField : GUILayoutAction
    {
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Float Variable to show in the edit field.")]
        public FsmFloat floatVariable;
        [HutongGames.PlayMaker.Tooltip("Optional GUIStyle in the active GUISKin.")]
        public FsmString style;
        [HutongGames.PlayMaker.Tooltip("Optional event to send when the value changes.")]
        public FsmEvent changedEvent;

        public override void OnGUI()
        {
            bool changed = GUI.changed;
            GUI.changed = false;
            this.floatVariable.Value = string.IsNullOrEmpty(this.style.Value) ? float.Parse(GUILayout.TextField(this.floatVariable.Value.ToString(), base.LayoutOptions)) : float.Parse(GUILayout.TextField(this.floatVariable.Value.ToString(), this.style.Value, base.LayoutOptions));
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
            this.floatVariable = null;
            this.style = "";
            this.changedEvent = null;
        }
    }
}

