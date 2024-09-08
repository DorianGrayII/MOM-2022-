namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("GUILayout Label for an Int Variable.")]
    public class GUILayoutIntLabel : GUILayoutAction
    {
        [HutongGames.PlayMaker.Tooltip("Text to put before the int variable.")]
        public FsmString prefix;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Int variable to display.")]
        public FsmInt intVariable;
        [HutongGames.PlayMaker.Tooltip("Optional GUIStyle in the active GUISKin.")]
        public FsmString style;

        public override void OnGUI()
        {
            if (string.IsNullOrEmpty(this.style.Value))
            {
                GUILayout.Label(new GUIContent(this.prefix.Value + this.intVariable.Value.ToString()), base.LayoutOptions);
            }
            else
            {
                GUILayout.Label(new GUIContent(this.prefix.Value + this.intVariable.Value.ToString()), this.style.Value, base.LayoutOptions);
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.prefix = "";
            this.style = "";
            this.intVariable = null;
        }
    }
}

