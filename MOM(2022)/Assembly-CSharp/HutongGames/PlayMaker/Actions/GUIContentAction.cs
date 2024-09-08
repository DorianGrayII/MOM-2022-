namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [HutongGames.PlayMaker.Tooltip("GUI base action - don't use!")]
    public abstract class GUIContentAction : GUIAction
    {
        public FsmTexture image;
        public FsmString text;
        public FsmString tooltip;
        public FsmString style;
        internal GUIContent content;

        protected GUIContentAction()
        {
        }

        public override void OnGUI()
        {
            base.OnGUI();
            this.content = new GUIContent(this.text.Value, this.image.get_Value(), this.tooltip.Value);
        }

        public override void Reset()
        {
            base.Reset();
            this.image = null;
            this.text = "";
            this.tooltip = "";
            this.style = "";
        }
    }
}

