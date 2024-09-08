using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Adds a 2d force to a Game Object. Use Vector2 variable and/or Float variables for each axis.")]
    public class AddForce2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField]
        [CheckForComponent(typeof(Rigidbody2D))]
        [Tooltip("The GameObject to apply the force to.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Option for applying the force")]
        public ForceMode2D forceMode;

        [UIHint(UIHint.Variable)]
        [Tooltip("Optionally apply the force at a position on the object. This will also add some torque. The position is often returned from MousePick or GetCollision2dInfo actions.")]
        public FsmVector2 atPosition;

        [UIHint(UIHint.Variable)]
        [Tooltip("A Vector2 force to add. Optionally override any axis with the X, Y parameters.")]
        public FsmVector2 vector;

        [Tooltip("Force along the X axis. To leave unchanged, set to 'None'.")]
        public FsmFloat x;

        [Tooltip("Force along the Y axis. To leave unchanged, set to 'None'.")]
        public FsmFloat y;

        [Tooltip("A Vector3 force to add. z is ignored")]
        public FsmVector3 vector3;

        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = null;
            this.atPosition = new FsmVector2
            {
                UseVariable = true
            };
            this.forceMode = ForceMode2D.Force;
            this.vector = null;
            this.vector3 = new FsmVector3
            {
                UseVariable = true
            };
            this.x = new FsmFloat
            {
                UseVariable = true
            };
            this.y = new FsmFloat
            {
                UseVariable = true
            };
            this.everyFrame = false;
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleFixedUpdate = true;
        }

        public override void OnEnter()
        {
            this.DoAddForce();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            this.DoAddForce();
        }

        private void DoAddForce()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                Vector2 force = (this.vector.IsNone ? new Vector2(this.x.Value, this.y.Value) : this.vector.Value);
                if (!this.vector3.IsNone)
                {
                    force.x = this.vector3.Value.x;
                    force.y = this.vector3.Value.y;
                }
                if (!this.x.IsNone)
                {
                    force.x = this.x.Value;
                }
                if (!this.y.IsNone)
                {
                    force.y = this.y.Value;
                }
                if (!this.atPosition.IsNone)
                {
                    base.rigidbody2d.AddForceAtPosition(force, this.atPosition.Value, this.forceMode);
                }
                else
                {
                    base.rigidbody2d.AddForce(force, this.forceMode);
                }
            }
        }
    }
}
