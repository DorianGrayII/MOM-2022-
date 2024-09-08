namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Convert)]
    [Tooltip("Converts an Integer value to a Float value.")]
    public class ConvertIntToFloat : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The Integer variable to convert to a float.")]
        public FsmInt intVariable;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a Float variable.")]
        public FsmFloat floatVariable;

        [Tooltip("Repeat every frame. Useful if the Integer variable is changing.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.intVariable = null;
            this.floatVariable = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoConvertIntToFloat();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoConvertIntToFloat();
        }

        private void DoConvertIntToFloat()
        {
            this.floatVariable.Value = this.intVariable.Value;
        }
    }
}
