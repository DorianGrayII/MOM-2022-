using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GUILayout)]
    [Tooltip("GUILayout Repeat Button. Sends an Event while pressed. Optionally store the button state in a Bool Variable.")]
    public class GUILayoutRepeatButton : GUILayoutAction
    {
        public FsmEvent sendEvent;

        [UIHint(UIHint.Variable)]
        public FsmBool storeButtonState;

        public FsmTexture image;

        public FsmString text;

        public FsmString tooltip;

        public FsmString style;

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

        public override void OnGUI()
        {
            bool flag = ((!string.IsNullOrEmpty(this.style.Value)) ? GUILayout.RepeatButton(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions) : GUILayout.RepeatButton(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), base.LayoutOptions));
            if (flag)
            {
                base.Fsm.Event(this.sendEvent);
            }
            this.storeButtonState.Value = flag;
        }
    }
}
