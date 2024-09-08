﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Convert), HutongGames.PlayMaker.Tooltip("Converts a Float value to an Integer value.")]
    public class ConvertFloatToInt : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The Float variable to convert to an integer.")]
        public FsmFloat floatVariable;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the result in an Integer variable.")]
        public FsmInt intVariable;
        public FloatRounding rounding;
        public bool everyFrame;

        private void DoConvertFloatToInt()
        {
            switch (this.rounding)
            {
                case FloatRounding.RoundDown:
                    this.intVariable.Value = Mathf.FloorToInt(this.floatVariable.Value);
                    return;

                case FloatRounding.RoundUp:
                    this.intVariable.Value = Mathf.CeilToInt(this.floatVariable.Value);
                    return;

                case FloatRounding.Nearest:
                    this.intVariable.Value = Mathf.RoundToInt(this.floatVariable.Value);
                    return;
            }
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

        public override void Reset()
        {
            this.floatVariable = null;
            this.intVariable = null;
            this.rounding = FloatRounding.Nearest;
            this.everyFrame = false;
        }

        public enum FloatRounding
        {
            RoundDown,
            RoundUp,
            Nearest
        }
    }
}

