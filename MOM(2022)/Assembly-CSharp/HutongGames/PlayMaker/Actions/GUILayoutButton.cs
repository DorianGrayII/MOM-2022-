namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("GUILayout Button. Sends an Event when pressed. Optionally stores the button state in a Bool Variable.")]
    public class GUILayoutButton : GUILayoutAction
    {
        public FsmEvent sendEvent;
        [UIHint(UIHint.Variable)]
        public FsmBool storeButtonState;
        public FsmTexture image;
        public FsmString text;
        public FsmString tooltip;
        public FsmString style;

        public override void OnGUI()
        {
            bool flag = !string.IsNullOrEmpty(this.style.Value) ? GUILayout.Button(new GUIContent(this.text.Value, this.image.get_Value(), this.tooltip.Value), this.style.Value, base.LayoutOptions) : GUILayout.Button(new GUIContent(this.text.Value, this.image.get_Value(), this.tooltip.Value), base.LayoutOptions);
            if (flag)
            {
                base.Fsm.Event(this.sendEvent);
            }
            if (this.storeButtonState != null)
            {
                this.storeButtonState.Value = flag;
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.sendEvent = null;
            this.storeButtonState = null;
            this.text = "";
            this.image = null;
            this.tooltip = "";
            this.style = "";
        }
    }
}

