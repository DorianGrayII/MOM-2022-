namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Math), HutongGames.PlayMaker.Tooltip("Clamps the value of Float Variable to a Min/Max range.")]
    public class FloatClamp : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Float variable to clamp.")]
        public FsmFloat floatVariable;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The minimum value.")]
        public FsmFloat minValue;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The maximum value.")]
        public FsmFloat maxValue;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the float variable is changing.")]
        public bool everyFrame;

        private void DoClamp()
        {
            this.floatVariable.Value = Mathf.Clamp(this.floatVariable.Value, this.minValue.Value, this.maxValue.Value);
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

        public override void Reset()
        {
            this.floatVariable = null;
            this.minValue = null;
            this.maxValue = null;
            this.everyFrame = false;
        }
    }
}

