using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Math)]
    [Tooltip("Performs math operation on 2 Integers: Add, Subtract, Multiply, Divide, Min, Max.")]
    public class IntOperator : FsmStateAction
    {
        public enum Operation
        {
            Add = 0,
            Subtract = 1,
            Multiply = 2,
            Divide = 3,
            Min = 4,
            Max = 5
        }

        [RequiredField]
        public FsmInt integer1;

        [RequiredField]
        public FsmInt integer2;

        public Operation operation;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmInt storeResult;

        public bool everyFrame;

        public override void Reset()
        {
            this.integer1 = null;
            this.integer2 = null;
            this.operation = Operation.Add;
            this.storeResult = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoIntOperator();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoIntOperator();
        }

        private void DoIntOperator()
        {
            int value = this.integer1.Value;
            int value2 = this.integer2.Value;
            switch (this.operation)
            {
            case Operation.Add:
                this.storeResult.Value = value + value2;
                break;
            case Operation.Subtract:
                this.storeResult.Value = value - value2;
                break;
            case Operation.Multiply:
                this.storeResult.Value = value * value2;
                break;
            case Operation.Divide:
                this.storeResult.Value = value / value2;
                break;
            case Operation.Min:
                this.storeResult.Value = Mathf.Min(value, value2);
                break;
            case Operation.Max:
                this.storeResult.Value = Mathf.Max(value, value2);
                break;
            }
        }
    }
}
