namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Character)]
    [Tooltip("Gets info on the last Character Controller collision and store in variables.")]
    public class GetControllerHitInfo : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the GameObject hit in the last collision.")]
        public FsmGameObject gameObjectHit;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the contact point of the last collision in world coordinates.")]
        public FsmVector3 contactPoint;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the normal of the last collision.")]
        public FsmVector3 contactNormal;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the direction of the last move before the collision.")]
        public FsmVector3 moveDirection;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the distance of the last move before the collision.")]
        public FsmFloat moveLength;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the physics material of the Game Object Hit. Useful for triggering different effects. Audio, particles...")]
        public FsmString physicsMaterialName;

        public override void Reset()
        {
            this.gameObjectHit = null;
            this.contactPoint = null;
            this.contactNormal = null;
            this.moveDirection = null;
            this.moveLength = null;
            this.physicsMaterialName = null;
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleControllerColliderHit = true;
        }

        private void StoreTriggerInfo()
        {
            if (base.Fsm.ControllerCollider != null)
            {
                this.gameObjectHit.Value = base.Fsm.ControllerCollider.gameObject;
                this.contactPoint.Value = base.Fsm.ControllerCollider.point;
                this.contactNormal.Value = base.Fsm.ControllerCollider.normal;
                this.moveDirection.Value = base.Fsm.ControllerCollider.moveDirection;
                this.moveLength.Value = base.Fsm.ControllerCollider.moveLength;
                this.physicsMaterialName.Value = base.Fsm.ControllerCollider.collider.material.name;
            }
        }

        public override void OnEnter()
        {
            this.StoreTriggerInfo();
            base.Finish();
        }

        public override string ErrorCheck()
        {
            return ActionHelpers.CheckPhysicsSetup(base.Owner);
        }
    }
}
