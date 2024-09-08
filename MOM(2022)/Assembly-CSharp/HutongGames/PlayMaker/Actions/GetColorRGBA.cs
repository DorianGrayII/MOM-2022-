namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Color), Tooltip("Get the RGBA channels of a Color Variable and store them in Float Variables.")]
    public class GetColorRGBA : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The Color variable.")]
        public FsmColor color;
        [UIHint(UIHint.Variable), Tooltip("Store the red channel in a float variable.")]
        public FsmFloat storeRed;
        [UIHint(UIHint.Variable), Tooltip("Store the green channel in a float variable.")]
        public FsmFloat storeGreen;
        [UIHint(UIHint.Variable), Tooltip("Store the blue channel in a float variable.")]
        public FsmFloat storeBlue;
        [UIHint(UIHint.Variable), Tooltip("Store the alpha channel in a float variable.")]
        public FsmFloat storeAlpha;
        [Tooltip("Repeat every frame. Useful if the color variable is changing.")]
        public bool everyFrame;

        private void DoGetColorRGBA()
        {
            if (!this.color.IsNone)
            {
                this.storeRed.Value = this.color.get_Value().r;
                this.storeGreen.Value = this.color.get_Value().g;
                this.storeBlue.Value = this.color.get_Value().b;
                this.storeAlpha.Value = this.color.get_Value().a;
            }
        }

        public override void OnEnter()
        {
            this.DoGetColorRGBA();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetColorRGBA();
        }

        public override void Reset()
        {
            this.color = null;
            this.storeRed = null;
            this.storeGreen = null;
            this.storeBlue = null;
            this.storeAlpha = null;
            this.everyFrame = false;
        }
    }
}

