namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Math), HutongGames.PlayMaker.Tooltip("Performs math operations on 2 Floats: Add, Subtract, Multiply, Divide, Min, Max.")]
    public class FloatOperator : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The first float.")]
        public FsmFloat float1;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The second float.")]
        public FsmFloat float2;
        [HutongGames.PlayMaker.Tooltip("The math operation to perform on the floats.")]
        public Operation operation;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the result of the operation in a float variable.")]
        public FsmFloat storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the variables are changing.")]
        public bool everyFrame;

        private void DoFloatOperator()
        {
            float a = this.float1.Value;
            float b = this.float2.Value;
            switch (this.operation)
            {
                case Operation.Add:
                    this.storeResult.Value = a + b;
                    return;

                case Operation.Subtract:
                    this.storeResult.Value = a - b;
                    return;

                case Operation.Multiply:
                    this.storeResult.Value = a * b;
                    return;

                case Operation.Divide:
                    this.storeResult.Value = a / b;
                    return;

                case Operation.Min:
                    this.storeResult.Value = Mathf.Min(a, b);
                    return;

                case Operation.Max:
                    this.storeResult.Value = Mathf.Max(a, b);
                    return;
            }
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

        public override void Reset()
        {
            this.float1 = null;
            this.float2 = null;
            this.operation = Operation.Add;
            this.storeResult = null;
            this.everyFrame = false;
        }

        public enum Operation
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Min,
            Max
        }
    }
}

