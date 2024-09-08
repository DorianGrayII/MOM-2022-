namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Sets the gravity vector, or individual axis.")]
    public class SetGravity : FsmStateAction
    {
        public FsmVector3 vector;
        public FsmFloat x;
        public FsmFloat y;
        public FsmFloat z;
        public bool everyFrame;

        private void DoSetGravity()
        {
            Vector3 vector = this.vector.get_Value();
            if (!this.x.IsNone)
            {
                vector.x = this.x.Value;
            }
            if (!this.y.IsNone)
            {
                vector.y = this.y.Value;
            }
            if (!this.z.IsNone)
            {
                vector.z = this.z.Value;
            }
            Physics.gravity = vector;
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
            FsmFloat num3 = new FsmFloat();
            num3.UseVariable = true;
            this.z = num3;
            this.everyFrame = false;
        }
    }
}

