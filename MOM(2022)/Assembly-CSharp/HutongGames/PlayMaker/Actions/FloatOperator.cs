using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Math)]
    [Tooltip("Performs math operations on 2 Floats: Add, Subtract, Multiply, Divide, Min, Max.")]
    public class FloatOperator : FsmStateAction
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
        [Tooltip("The first float.")]
        public FsmFloat float1;

        [RequiredField]
        [Tooltip("The second float.")]
        public FsmFloat float2;

        [Tooltip("The math operation to perform on the floats.")]
        public Operation operation;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result of the operation in a float variable.")]
        public FsmFloat storeResult;

        [Tooltip("Repeat every frame. Useful if the variables are changing.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.float1 = null;
            this.float2 = null;
            this.operation = Operation.Add;
            this.storeResult = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoFloatOperator();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoFloatOperator();
        }

        private void DoFloatOperator()
        {
            float value = this.float1.Value;
            float value2 = this.float2.Value;
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
