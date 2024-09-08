namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Vector2)]
    [Tooltip("Get the XY channels of a Vector2 Variable and store them in Float Variables.")]
    public class GetVector2XY : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The vector2 source")]
        public FsmVector2 vector2Variable;

        [UIHint(UIHint.Variable)]
        [Tooltip("The x component")]
        public FsmFloat storeX;

        [UIHint(UIHint.Variable)]
        [Tooltip("The y component")]
        public FsmFloat storeY;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.vector2Variable = null;
            this.storeX = null;
            this.storeY = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoGetVector2XYZ();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetVector2XYZ();
        }

        private void DoGetVector2XYZ()
        {
            if (this.vector2Variable != null)
            {
                if (this.storeX != null)
                {
                    this.storeX.Value = this.vector2Variable.Value.x;
                }
                if (this.storeY != null)
                {
                    this.storeY.Value = this.vector2Variable.Value.y;
                }
            }
        }
    }
}
