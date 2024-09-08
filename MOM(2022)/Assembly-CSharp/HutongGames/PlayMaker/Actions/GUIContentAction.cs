using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("GUI base action - don't use!")]
    public abstract class GUIContentAction : GUIAction
    {
        public FsmTexture image;

        public FsmString text;

        public FsmString tooltip;

        public FsmString style;

        internal GUIContent content;

        public override void Reset()
        {
            base.Reset();
            this.image = null;
            this.text = "";
            this.tooltip = "";
            this.style = "";
        }

        public override void OnGUI()
        {
            base.OnGUI();
            this.content = new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value);
        }
    }
}
