namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Physics), Tooltip("Gets info on the last collision event and store in variables. See Unity Physics docs.")]
    public class GetCollisionInfo : FsmStateAction
    {
        [UIHint(UIHint.Variable), Tooltip("Get the GameObject hit.")]
        public FsmGameObject gameObjectHit;
        [UIHint(UIHint.Variable), Tooltip("Get the relative velocity of the collision.")]
        public FsmVector3 relativeVelocity;
        [UIHint(UIHint.Variable), Tooltip("Get the relative speed of the collision. Useful for controlling reactions. E.g., selecting an appropriate sound fx.")]
        public FsmFloat relativeSpeed;
        [UIHint(UIHint.Variable), Tooltip("Get the world position of the collision contact. Useful for spawning effects etc.")]
        public FsmVector3 contactPoint;
        [UIHint(UIHint.Variable), Tooltip("Get the collision normal vector. Useful for aligning spawned effects etc.")]
        public FsmVector3 contactNormal;
        [UIHint(UIHint.Variable), Tooltip("Get the name of the physics material of the colliding GameObject. Useful for triggering different effects. Audio, particles...")]
        public FsmString physicsMaterialName;

        public override void OnEnter()
        {
            this.StoreCollisionInfo();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObjectHit = null;
            this.relativeVelocity = null;
            this.relativeSpeed = null;
            this.contactPoint = null;
            this.contactNormal = null;
            this.physicsMaterialName = null;
        }

        private void StoreCollisionInfo()
        {
            if (base.Fsm.get_CollisionInfo() != null)
            {
                this.gameObjectHit.set_Value(base.Fsm.get_CollisionInfo().gameObject);
                this.relativeSpeed.Value = base.Fsm.get_CollisionInfo().relativeVelocity.magnitude;
                this.relativeVelocity.set_Value(base.Fsm.get_CollisionInfo().relativeVelocity);
                this.physicsMaterialName.Value = base.Fsm.get_CollisionInfo().collider.material.name;
                if ((base.Fsm.get_CollisionInfo().contacts != null) && (base.Fsm.get_CollisionInfo().contacts.Length != 0))
                {
                    this.contactPoint.set_Value(base.Fsm.get_CollisionInfo().contacts[0].point);
                    this.contactNormal.set_Value(base.Fsm.get_CollisionInfo().contacts[0].normal);
                }
            }
        }
    }
}

