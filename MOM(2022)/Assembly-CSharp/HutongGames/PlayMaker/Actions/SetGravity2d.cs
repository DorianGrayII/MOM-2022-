namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Sets the gravity vector, or individual axis.")]
    public class SetGravity2d : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Gravity as Vector2.")]
        public FsmVector2 vector;
        [HutongGames.PlayMaker.Tooltip("Override the x value of the gravity")]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("Override the y value of the gravity")]
        public FsmFloat y;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
        public bool everyFrame;

        private void DoSetGravity()
        {
            Vector2 vector = this.vector.get_Value();
            if (!this.x.IsNone)
            {
                vector.x = this.x.Value;
            }
            if (!this.y.IsNone)
            {
                vector.y = this.y.Value;
            }
            Physics2D.gravity = vector;
        }

        public override void OnEnter()
        {
            this.DoSetGravity();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetGravity();
        }

        public override void Reset()
        {
            this.vector = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.x = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.y = num2;
            this.everyFrame = false;
        }
    }
}

