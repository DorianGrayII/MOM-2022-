namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Vector2), HutongGames.PlayMaker.Tooltip("Performs most possible operations on 2 Vector2: Dot product, Distance, Angle, Add, Subtract, Multiply, Divide, Min, Max")]
    public class Vector2Operator : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The first vector")]
        public FsmVector2 vector1;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The second vector")]
        public FsmVector2 vector2;
        [HutongGames.PlayMaker.Tooltip("The operation")]
        public Vector2Operation operation = Vector2Operation.Add;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The Vector2 result when it applies.")]
        public FsmVector2 storeVector2Result;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The float result when it applies")]
        public FsmFloat storeFloatResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
        public bool everyFrame;

        private void DoVector2Operator()
        {
            Vector2 lhs = this.vector1.get_Value();
            Vector2 rhs = this.vector2.get_Value();
            switch (this.operation)
            {
                case Vector2Operation.DotProduct:
                    this.storeFloatResult.Value = Vector2.Dot(lhs, rhs);
                    return;

                case Vector2Operation.Distance:
                    this.storeFloatResult.Value = Vector2.Distance(lhs, rhs);
                    return;

                case Vector2Operation.Angle:
                    this.storeFloatResult.Value = Vector2.Angle(lhs, rhs);
                    return;

                case Vector2Operation.Add:
                    this.storeVector2Result.set_Value(lhs + rhs);
                    return;

                case Vector2Operation.Subtract:
                    this.storeVector2Result.set_Value(lhs - rhs);
                    return;

                case Vector2Operation.Multiply:
                {
                    Vector2 zero = Vector2.zero;
                    zero.x = lhs.x * rhs.x;
                    zero.y = lhs.y * rhs.y;
                    this.storeVector2Result.set_Value(zero);
                    return;
                }
                case Vector2Operation.Divide:
                {
                    Vector2 zero = Vector2.zero;
                    zero.x = lhs.x / rhs.x;
                    zero.y = lhs.y / rhs.y;
                    this.storeVector2Result.set_Value(zero);
                    return;
                }
                case Vector2Operation.Min:
                    this.storeVector2Result.set_Value(Vector2.Min(lhs, rhs));
                    return;

                case Vector2Operation.Max:
                    this.storeVector2Result.set_Value(Vector2.Max(lhs, rhs));
                    return;
            }
        }

        public override void OnEnter()
        {
            this.DoVector2Operator();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoVector2Operator();
        }

        public override void Reset()
        {
            this.vector1 = null;
            this.vector2 = null;
            this.operation = Vector2Operation.Add;
            this.storeVector2Result = null;
            this.storeFloatResult = null;
            this.everyFrame = false;
        }

        public enum Vector2Operation
        {
            DotProduct,
            Distance,
            Angle,
            Add,
            Subtract,
            Multiply,
            Divide,
            Min,
            Max
        }
    }
}

