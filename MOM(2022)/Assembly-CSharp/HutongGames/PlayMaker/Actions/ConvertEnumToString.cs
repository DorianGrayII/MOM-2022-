namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Convert), Tooltip("Converts an Enum value to a String value.")]
    public class ConvertEnumToString : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The Enum variable to convert.")]
        public FsmEnum enumVariable;
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The String variable to store the converted value.")]
        public FsmString stringVariable;
        [Tooltip("Repeat every frame. Useful if the Enum variable is changing.")]
        public bool everyFrame;

        private void DoConvertEnumToString()
        {
            this.stringVariable.Value = (this.enumVariable.Value != 0) ? this.enumVariable.Value.ToString() : "";
        }

        public override void OnEnter()
        {
            this.DoConvertEnumToString();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoConvertEnumToString();
        }

        public override void Reset()
        {
            this.enumVariable = null;
            this.stringVariable = null;
            this.everyFrame = false;
        }
    }
}

