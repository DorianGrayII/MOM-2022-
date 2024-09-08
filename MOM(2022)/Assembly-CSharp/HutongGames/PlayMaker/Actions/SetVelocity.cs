namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Sets the Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody.")]
    public class SetVelocity : ComponentAction<Rigidbody>
    {
        [RequiredField, CheckForComponent(typeof(Rigidbody))]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable)]
        public FsmVector3 vector;
        public FsmFloat x;
        public FsmFloat y;
        public FsmFloat z;
        public Space space;
        public bool everyFrame;

        private void DoSetVelocity()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                Vector3 direction = !this.vector.IsNone ? this.vector.get_Value() : ((this.space == Space.World) ? base.rigidbody.velocity : ownerDefaultTarget.transform.InverseTransformDirection(base.rigidbody.velocity));
                if (!this.x.IsNone)
                {
                    direction.x = this.x.Value;
                }
                if (!this.y.IsNone)
                {
                    direction.y = this.y.Value;
                }
                if (!this.z.IsNone)
                {
                    direction.z = this.z.Value;
                }
                base.rigidbody.velocity = (this.space == Space.World) ? direction : ownerDefaultTarget.transform.TransformDirection(direction);
            }
        }

        public override void OnEnter()
        {
            this.DoSetVelocity();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            this.DoSetVelocity();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleFixedUpdate = true;
        }

        public override void Reset()
        {
            this.gameObject = null;
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
            this.space = Space.Self;
            this.everyFrame = false;
        }
    }
}

