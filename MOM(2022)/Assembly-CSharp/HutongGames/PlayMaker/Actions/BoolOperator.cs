namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Math)]
    [Tooltip("Performs boolean operations on 2 Bool Variables.")]
    public class BoolOperator : FsmStateAction
    {
        public enum Operation
        {
            AND = 0,
            NAND = 1,
            OR = 2,
            XOR = 3
        }

        [RequiredField]
        [Tooltip("The first Bool variable.")]
        public FsmBool bool1;

        [RequiredField]
        [Tooltip("The second Bool variable.")]
        public FsmBool bool2;

        [Tooltip("Boolean Operation.")]
        public Operation operation;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a Bool Variable.")]
        public FsmBool storeResult;

        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.bool1 = false;
            this.bool2 = false;
            this.operation = Operation.AND;
            this.storeResult = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoBoolOperator();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoBoolOperator();
        }

        private void DoBoolOperator()
        {
            bool value = this.bool1.Value;
            bool value2 = this.bool2.Value;
            switch (this.operation)
            {
            case Operation.AND:
                this.storeResult.Value = value && value2;
                break;
            case Operation.NAND:
                this.storeResult.Value = !(value && value2);
                break;
            case Operation.OR:
                this.storeResult.Value = value || value2;
                break;
            case Operation.XOR:
                this.storeResult.Value = value ^ value2;
                break;
            }
        }
    }
}
