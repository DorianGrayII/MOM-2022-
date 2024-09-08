using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Adds a relative 2d force to a Game Object. Use Vector2 variable and/or Float variables for each axis.")]
    public class AddRelativeForce2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField]
        [CheckForComponent(typeof(Rigidbody2D))]
        [Tooltip("The GameObject to apply the force to.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Option for applying the force")]
        public ForceMode2D forceMode;

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
            this.DoAddRelativeForce();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            this.DoAddRelativeForce();
        }

        private void DoAddRelativeForce()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                Vector2 relativeForce = (this.vector.IsNone ? new Vector2(this.x.Value, this.y.Value) : this.vector.Value);
                if (!this.vector3.IsNone)
                {
                    relativeForce.x = this.vector3.Value.x;
                    relativeForce.y = this.vector3.Value.y;
                }
                if (!this.x.IsNone)
                {
                    relativeForce.x = this.x.Value;
                }
                if (!this.y.IsNone)
                {
                    relativeForce.y = this.y.Value;
                }
                base.rigidbody2d.AddRelativeForce(relativeForce, this.forceMode);
            }
        }
    }
}
