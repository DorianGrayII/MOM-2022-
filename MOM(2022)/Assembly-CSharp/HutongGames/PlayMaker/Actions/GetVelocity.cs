namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Gets the Velocity of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable. NOTE: The Game Object must have a Rigid Body.")]
    public class GetVelocity : ComponentAction<Rigidbody>
    {
        [RequiredField, CheckForComponent(typeof(Rigidbody))]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable)]
        public FsmVector3 vector;
        [UIHint(UIHint.Variable)]
        public FsmFloat x;
        [UIHint(UIHint.Variable)]
        public FsmFloat y;
        [UIHint(UIHint.Variable)]
        public FsmFloat z;
        public Space space;
        public bool everyFrame;

        private void DoGetVelocity()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                Vector3 velocity = base.rigidbody.velocity;
                if (this.space == Space.Self)
                {
                    velocity = ownerDefaultTarget.transform.InverseTransformDirection(velocity);
                }
                this.vector.set_Value(velocity);
                this.x.Value = velocity.x;
                this.y.Value = velocity.y;
                this.z.Value = velocity.z;
            }
        }

        public override void OnEnter()
        {
            this.DoGetVelocity();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetVelocity();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.vector = null;
            this.x = null;
            this.y = null;
            this.z = null;
            this.space = Space.World;
            this.everyFrame = false;
        }
    }
}

