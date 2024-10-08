using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics)]
    [Tooltip("Sets the Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody.")]
    public class SetVelocity : ComponentAction<Rigidbody>
    {
        [RequiredField]
        [CheckForComponent(typeof(Rigidbody))]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        public FsmVector3 vector;

        public FsmFloat x;

        public FsmFloat y;

        public FsmFloat z;

        public Space space;

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
            this.z = new FsmFloat
            {
                UseVariable = true
            };
            this.space = Space.Self;
            this.everyFrame = false;
        }

        public override void OnPreprocess()
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
                Vector3 vector = ((!this.vector.IsNone) ? this.vector.Value : ((this.space == Space.World) ? base.rigidbody.velocity : ownerDefaultTarget.transform.InverseTransformDirection(base.rigidbody.velocity)));
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
                base.rigidbody.velocity = ((this.space == Space.World) ? vector : ownerDefaultTarget.transform.TransformDirection(vector));
            }
        }
    }
}
