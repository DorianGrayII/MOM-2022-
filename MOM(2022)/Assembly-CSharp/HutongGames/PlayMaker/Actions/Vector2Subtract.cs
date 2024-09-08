namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Vector2)]
    [Tooltip("Subtracts a Vector2 value from a Vector2 variable.")]
    public class Vector2Subtract : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The Vector2 operand")]
        public FsmVector2 vector2Variable;

        [RequiredField]
        [Tooltip("The vector2 to subtract with")]
        public FsmVector2 subtractVector;

        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        public override void Reset()
        {
            this.vector2Variable = null;
            this.subtractVector = new FsmVector2
            {
                UseVariable = true
            };
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.vector2Variable.Value = this.vector2Variable.Value - this.subtractVector.Value;
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.vector2Variable.Value = this.vector2Variable.Value - this.subtractVector.Value;
        }
    }
}
