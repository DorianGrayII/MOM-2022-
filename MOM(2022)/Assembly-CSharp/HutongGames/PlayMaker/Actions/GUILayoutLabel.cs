using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GUILayout)]
    [Tooltip("GUILayout Label.")]
    public class GUILayoutLabel : GUILayoutAction
    {
        public FsmTexture image;

        public FsmString text;

        public FsmString tooltip;

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
                GUILayout.Label(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), base.LayoutOptions);
            }
            else
            {
                GUILayout.Label(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
            }
        }
    }
}
