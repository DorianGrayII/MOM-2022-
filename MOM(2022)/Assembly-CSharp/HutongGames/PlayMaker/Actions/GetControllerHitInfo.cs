namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Character), Tooltip("Gets info on the last Character Controller collision and store in variables.")]
    public class GetControllerHitInfo : FsmStateAction
    {
        [UIHint(UIHint.Variable), Tooltip("Store the GameObject hit in the last collision.")]
        public FsmGameObject gameObjectHit;
        [UIHint(UIHint.Variable), Tooltip("Store the contact point of the last collision in world coordinates.")]
        public FsmVector3 contactPoint;
        [UIHint(UIHint.Variable), Tooltip("Store the normal of the last collision.")]
        public FsmVector3 contactNormal;
        [UIHint(UIHint.Variable), Tooltip("Store the direction of the last move before the collision.")]
        public FsmVector3 moveDirection;
        [UIHint(UIHint.Variable), Tooltip("Store the distance of the last move before the collision.")]
        public FsmFloat moveLength;
        [UIHint(UIHint.Variable), Tooltip("Store the physics material of the Game Object Hit. Useful for triggering different effects. Audio, particles...")]
        public FsmString physicsMaterialName;

        public override string ErrorCheck()
        {
            return ActionHelpers.CheckPhysicsSetup(base.get_Owner());
        }

        public override void OnEnter()
        {
            this.StoreTriggerInfo();
            base.Finish();
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleControllerColliderHit = true;
        }

        public override void Reset()
        {
            this.gameObjectHit = null;
            this.contactPoint = null;
            this.contactNormal = null;
            this.moveDirection = null;
            this.moveLength = null;
            this.physicsMaterialName = null;
        }

        private void StoreTriggerInfo()
        {
            if (base.Fsm.get_ControllerCollider() != null)
            {
                this.gameObjectHit.set_Value(base.Fsm.get_ControllerCollider().gameObject);
                this.contactPoint.set_Value(base.Fsm.get_ControllerCollider().point);
                this.contactNormal.set_Value(base.Fsm.get_ControllerCollider().normal);
                this.moveDirection.set_Value(base.Fsm.get_ControllerCollider().moveDirection);
                this.moveLength.Value = base.Fsm.get_ControllerCollider().moveLength;
                this.physicsMaterialName.Value = base.Fsm.get_ControllerCollider().collider.material.name;
            }
        }
    }
}

