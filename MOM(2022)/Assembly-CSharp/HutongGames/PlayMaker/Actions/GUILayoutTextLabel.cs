using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GUILayout)]
    [Tooltip("GUILayout Label for simple text.")]
    public class GUILayoutTextLabel : GUILayoutAction
    {
        [Tooltip("Text to display.")]
        public FsmString text;

        [Tooltip("Optional GUIStyle in the active GUISkin.")]
        public FsmString style;

        public override void Reset()
        {
            base.Reset();
            this.text = "";
            this.style = "";
        }

        public override void OnGUI()
        {
            if (string.IsNullOrEmpty(this.style.Value))
            {
                GUILayout.Label(new GUIContent(this.text.Value), base.LayoutOptions);
            }
            else
            {
                GUILayout.Label(new GUIContent(this.text.Value), this.style.Value, base.LayoutOptions);
            }
        }
    }
}
