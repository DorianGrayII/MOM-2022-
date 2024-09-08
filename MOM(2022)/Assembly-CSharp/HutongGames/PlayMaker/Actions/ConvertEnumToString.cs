namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Convert)]
    [Tooltip("Converts an Enum value to a String value.")]
    public class ConvertEnumToString : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The Enum variable to convert.")]
        public FsmEnum enumVariable;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The String variable to store the converted value.")]
        public FsmString stringVariable;

        [Tooltip("Repeat every frame. Useful if the Enum variable is changing.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.enumVariable = null;
            this.stringVariable = null;
            this.everyFrame = false;
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

        private void DoConvertEnumToString()
        {
            this.stringVariable.Value = ((this.enumVariable.Value != null) ? this.enumVariable.Value.ToString() : "");
        }
    }
}
