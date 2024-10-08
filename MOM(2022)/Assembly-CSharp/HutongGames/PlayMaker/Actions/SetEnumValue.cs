namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Enum)]
    [Tooltip("Sets the value of an Enum Variable.")]
    public class SetEnumValue : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("The Enum Variable to set.")]
        public FsmEnum enumVariable;

        [MatchFieldType("enumVariable")]
        [Tooltip("The Enum value to set the variable to.")]
        public FsmEnum enumValue;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.enumVariable = null;
            this.enumValue = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetEnumValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetEnumValue();
        }

        private void DoSetEnumValue()
        {
            this.enumVariable.Value = this.enumValue.Value;
        }
    }
}
