namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Math)]
    [Tooltip("Multiplies one Float by another.")]
    public class FloatMultiply : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The float variable to multiply.")]
        public FsmFloat floatVariable;

        [RequiredField]
        [Tooltip("Multiply the float variable by this value.")]
        public FsmFloat multiplyBy;

        [Tooltip("Repeat every frame. Useful if the variables are changing.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.floatVariable = null;
            this.multiplyBy = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.floatVariable.Value *= this.multiplyBy.Value;
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.floatVariable.Value *= this.multiplyBy.Value;
        }
    }
}
