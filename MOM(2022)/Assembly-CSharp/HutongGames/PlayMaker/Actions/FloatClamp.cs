using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Math)]
    [Tooltip("Clamps the value of Float Variable to a Min/Max range.")]
    public class FloatClamp : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Float variable to clamp.")]
        public FsmFloat floatVariable;

        [RequiredField]
        [Tooltip("The minimum value.")]
        public FsmFloat minValue;

        [RequiredField]
        [Tooltip("The maximum value.")]
        public FsmFloat maxValue;

        [Tooltip("Repeat every frame. Useful if the float variable is changing.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.floatVariable = null;
            this.minValue = null;
            this.maxValue = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoClamp();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoClamp();
        }

        private void DoClamp()
        {
            this.floatVariable.Value = Mathf.Clamp(this.floatVariable.Value, this.minValue.Value, this.maxValue.Value);
        }
    }
}
