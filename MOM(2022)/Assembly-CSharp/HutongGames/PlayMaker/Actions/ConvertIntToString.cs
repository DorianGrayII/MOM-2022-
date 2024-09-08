﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Convert), Tooltip("Converts an Integer value to a String value with an optional format.")]
    public class ConvertIntToString : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The Int variable to convert.")]
        public FsmInt intVariable;
        [RequiredField, UIHint(UIHint.Variable), Tooltip("A String variable to store the converted value.")]
        public FsmString stringVariable;
        [Tooltip("Optional Format, allows for leading zeros. E.g., 0000")]
        public FsmString format;
        [Tooltip("Repeat every frame. Useful if the Int variable is changing.")]
        public bool everyFrame;

        private void DoConvertIntToString()
        {
            if (!this.format.IsNone && !string.IsNullOrEmpty(this.format.Value))
            {
                this.stringVariable.Value = this.intVariable.Value.ToString(this.format.Value);
            }
            else
            {
                this.stringVariable.Value = this.intVariable.Value.ToString();
            }
        }

        public override void OnEnter()
        {
            this.DoConvertIntToString();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoConvertIntToString();
        }

        public override void Reset()
        {
            this.intVariable = null;
            this.stringVariable = null;
            this.everyFrame = false;
            this.format = null;
        }
    }
}

