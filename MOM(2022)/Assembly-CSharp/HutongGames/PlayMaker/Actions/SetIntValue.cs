namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Math)]
    [Tooltip("Sets the value of an Integer Variable.")]
    public class SetIntValue : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Int Variable to Set")]
        public FsmInt intVariable;

        [RequiredField]
        [Tooltip("Int Value")]
        public FsmInt intValue;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.intVariable = null;
            this.intValue = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.intVariable.Value = this.intValue.Value;
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.intVariable.Value = this.intValue.Value;
        }
    }
}
