using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GUILayout)]
    [Tooltip("Begins a vertical control group. The group must be closed with GUILayoutEndVertical action.")]
    public class GUILayoutBeginVertical : GUILayoutAction
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
            GUILayout.BeginVertical(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
        }
    }
}
