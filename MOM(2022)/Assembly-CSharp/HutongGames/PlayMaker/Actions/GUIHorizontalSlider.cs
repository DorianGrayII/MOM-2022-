namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("GUI Horizontal Slider connected to a Float Variable.")]
    public class GUIHorizontalSlider : GUIAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmFloat floatVariable;
        [RequiredField]
        public FsmFloat leftValue;
        [RequiredField]
        public FsmFloat rightValue;
        public FsmString sliderStyle;
        public FsmString thumbStyle;

        public override void OnGUI()
        {
            base.OnGUI();
            if (this.floatVariable != null)
            {
                this.floatVariable.Value = GUI.HorizontalSlider(base.rect, this.floatVariable.Value, this.leftValue.Value, this.rightValue.Value, (this.sliderStyle.Value != "") ? this.sliderStyle.Value : "horizontalslider", (this.thumbStyle.Value != "") ? this.thumbStyle.Value : "horizontalsliderthumb");
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.floatVariable = null;
            this.leftValue = 0f;
            this.rightValue = 100f;
            this.sliderStyle = "horizontalslider";
            this.thumbStyle = "horizontalsliderthumb";
        }
    }
}

