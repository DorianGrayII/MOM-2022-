namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Color), HutongGames.PlayMaker.Tooltip("Sets the RGBA channels of a Color Variable. To leave any channel unchanged, set variable to 'None'.")]
    public class SetColorRGBA : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmColor colorVariable;
        [HasFloatSlider(0f, 1f)]
        public FsmFloat red;
        [HasFloatSlider(0f, 1f)]
        public FsmFloat green;
        [HasFloatSlider(0f, 1f)]
        public FsmFloat blue;
        [HasFloatSlider(0f, 1f)]
        public FsmFloat alpha;
        public bool everyFrame;

        private void DoSetColorRGBA()
        {
            if (this.colorVariable != null)
            {
                Color color = this.colorVariable.get_Value();
                if (!this.red.IsNone)
                {
                    color.r = this.red.Value;
                }
                if (!this.green.IsNone)
                {
                    color.g = this.green.Value;
                }
                if (!this.blue.IsNone)
                {
                    color.b = this.blue.Value;
                }
                if (!this.alpha.IsNone)
                {
                    color.a = this.alpha.Value;
                }
                this.colorVariable.set_Value(color);
            }
        }

        public override void OnEnter()
        {
            this.DoSetColorRGBA();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetColorRGBA();
        }

        public override void Reset()
        {
            this.colorVariable = null;
            this.red = 0f;
            this.green = 0f;
            this.blue = 0f;
            this.alpha = 1f;
            this.everyFrame = false;
        }
    }
}

