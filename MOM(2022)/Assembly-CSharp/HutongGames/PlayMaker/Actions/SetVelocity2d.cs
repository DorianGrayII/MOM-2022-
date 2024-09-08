using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Sets the 2d Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody 2D.")]
    public class SetVelocity2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField]
        [CheckForComponent(typeof(Rigidbody2D))]
        [Tooltip("The GameObject with the Rigidbody2D attached")]
        public FsmOwnerDefault gameObject;

        [Tooltip("A Vector2 value for the velocity")]
        public FsmVector2 vector;

        [Tooltip("The y value of the velocity. Overrides 'Vector' x value if set")]
        public FsmFloat x;

        [Tooltip("The y value of the velocity. Overrides 'Vector' y value if set")]
        public FsmFloat y;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = null;
            this.vector = null;
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

        public override void Awake()
        {
            base.Fsm.HandleFixedUpdate = true;
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

        private void DoSetVelocity()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                Vector2 velocity = ((!this.vector.IsNone) ? this.vector.Value : base.rigidbody2d.velocity);
                if (!this.x.IsNone)
                {
                    velocity.x = this.x.Value;
                }
                if (!this.y.IsNone)
                {
                    velocity.y = this.y.Value;
                }
                base.rigidbody2d.velocity = velocity;
            }
        }
    }
}
