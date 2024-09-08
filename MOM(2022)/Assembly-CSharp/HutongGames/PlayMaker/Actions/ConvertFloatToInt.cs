using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Convert)]
    [Tooltip("Converts a Float value to an Integer value.")]
    public class ConvertFloatToInt : FsmStateAction
    {
        public enum FloatRounding
        {
            RoundDown = 0,
            RoundUp = 1,
            Nearest = 2
        }

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The Float variable to convert to an integer.")]
        public FsmFloat floatVariable;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in an Integer variable.")]
        public FsmInt intVariable;

        public FloatRounding rounding;

        public bool everyFrame;

        public override void Reset()
        {
            this.floatVariable = null;
            this.intVariable = null;
            this.rounding = FloatRounding.Nearest;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoConvertFloatToInt();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoConvertFloatToInt();
        }

        private void DoConvertFloatToInt()
        {
            switch (this.rounding)
            {
            case FloatRounding.Nearest:
                this.intVariable.Value = Mathf.RoundToInt(this.floatVariable.Value);
                break;
            case FloatRounding.RoundDown:
                this.intVariable.Value = Mathf.FloorToInt(this.floatVariable.Value);
                break;
            case FloatRounding.RoundUp:
                this.intVariable.Value = Mathf.CeilToInt(this.floatVariable.Value);
                break;
            }
        }
    }
}
