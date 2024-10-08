using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Gets the 2d Speed of a Game Object and stores it in a Float Variable. NOTE: The Game Object must have a rigid body 2D.")]
    public class GetSpeed2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField]
        [CheckForComponent(typeof(Rigidbody2D))]
        [Tooltip("The GameObject with the Rigidbody2D attached")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The speed, or in technical terms: velocity magnitude")]
        public FsmFloat storeResult;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = null;
            this.storeResult = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoGetSpeed();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetSpeed();
        }

        private void DoGetSpeed()
        {
            if (!this.storeResult.IsNone)
            {
                GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
                if (base.UpdateCache(ownerDefaultTarget))
                {
                    this.storeResult.Value = base.rigidbody2d.velocity.magnitude;
                }
            }
        }
    }
}
