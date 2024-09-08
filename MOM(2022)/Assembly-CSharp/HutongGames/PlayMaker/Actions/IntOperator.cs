namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Math), HutongGames.PlayMaker.Tooltip("Performs math operation on 2 Integers: Add, Subtract, Multiply, Divide, Min, Max.")]
    public class IntOperator : FsmStateAction
    {
        [RequiredField]
        public FsmInt integer1;
        [RequiredField]
        public FsmInt integer2;
        public Operation operation;
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmInt storeResult;
        public bool everyFrame;

        private void DoIntOperator()
        {
            int a = this.integer1.Value;
            int b = this.integer2.Value;
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

        public override void Reset()
        {
            this.integer1 = null;
            this.integer2 = null;
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

