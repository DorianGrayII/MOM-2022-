namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Gets the 2d Velocity of a Game Object and stores it in a Vector2 Variable or each Axis in a Float Variable. NOTE: The Game Object must have a Rigid Body 2D.")]
    public class GetVelocity2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField, CheckForComponent(typeof(Rigidbody2D)), HutongGames.PlayMaker.Tooltip("The GameObject with the Rigidbody2D attached")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The velocity")]
        public FsmVector2 vector;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The x value of the velocity")]
        public FsmFloat x;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The y value of the velocity")]
        public FsmFloat y;
        [HutongGames.PlayMaker.Tooltip("The space reference to express the velocity")]
        public Space space;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoGetVelocity()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                Vector2 velocity = base.rigidbody2d.velocity;
                if (this.space == Space.Self)
                {
                    velocity = base.rigidbody2d.transform.InverseTransformDirection((Vector3) velocity);
                }
                this.vector.set_Value(velocity);
                this.x.Value = velocity.x;
                this.y.Value = velocity.y;
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
            this.space = Space.World;
            this.everyFrame = false;
        }
    }
}

