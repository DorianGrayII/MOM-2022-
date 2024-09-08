namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Convert), HutongGames.PlayMaker.Tooltip("Converts a Bool value to a Color.")]
    public class ConvertBoolToColor : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The Bool variable to test.")]
        public FsmBool boolVariable;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The Color variable to set based on the bool variable value.")]
        public FsmColor colorVariable;
        [HutongGames.PlayMaker.Tooltip("Color if Bool variable is false.")]
        public FsmColor falseColor;
        [HutongGames.PlayMaker.Tooltip("Color if Bool variable is true.")]
        public FsmColor trueColor;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
        public bool everyFrame;

        private void DoConvertBoolToColor()
        {
            this.colorVariable.set_Value(this.boolVariable.Value ? this.trueColor.get_Value() : this.falseColor.get_Value());
        }

        public override void OnEnter()
        {
            this.DoConvertBoolToColor();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoConvertBoolToColor();
        }

        public override void Reset()
        {
            this.boolVariable = null;
            this.colorVariable = null;
            this.falseColor = (FsmColor) Color.black;
            this.trueColor = (FsmColor) Color.white;
            this.everyFrame = false;
        }
    }
}

