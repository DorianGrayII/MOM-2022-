namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("GUILayout Label.")]
    public class GUILayoutLabel : GUILayoutAction
    {
        public FsmTexture image;
        public FsmString text;
        public FsmString tooltip;
        public FsmString style;

        public override void OnGUI()
        {
            if (string.IsNullOrEmpty(this.style.Value))
            {
                GUILayout.Label(new GUIContent(this.text.Value, this.image.get_Value(), this.tooltip.Value), base.LayoutOptions);
            }
            else
            {
                GUILayout.Label(new GUIContent(this.text.Value, this.image.get_Value(), this.tooltip.Value), this.style.Value, base.LayoutOptions);
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.text = "";
            this.image = null;
            this.tooltip = "";
            this.style = "";
        }
    }
}

