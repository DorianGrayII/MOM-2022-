using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GUILayout)]
    [Tooltip("GUILayout Box.")]
    public class GUILayoutBox : GUILayoutAction
    {
        [Tooltip("Image to display in the Box.")]
        public FsmTexture image;

        [Tooltip("Text to display in the Box.")]
        public FsmString text;

        [Tooltip("Optional Tooltip string.")]
        public FsmString tooltip;

        [Tooltip("Optional GUIStyle in the active GUISkin.")]
        public FsmString style;

        public override void Reset()
        {
            base.Reset();
            this.text = "";
            this.image = null;
            this.tooltip = "";
            this.style = "";
        }

        public override void OnGUI()
        {
            if (string.IsNullOrEmpty(this.style.Value))
            {
                GUILayout.Box(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), base.LayoutOptions);
            }
            else
            {
                GUILayout.Box(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
            }
        }
    }
}
