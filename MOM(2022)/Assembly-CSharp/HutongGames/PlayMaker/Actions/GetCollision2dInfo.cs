namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Gets info on the last collision 2D event and store in variables. See Unity and PlayMaker docs on Unity 2D physics.")]
    public class GetCollision2dInfo : FsmStateAction
    {
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the GameObject hit.")]
        public FsmGameObject gameObjectHit;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the relative velocity of the collision.")]
        public FsmVector3 relativeVelocity;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the relative speed of the collision. Useful for controlling reactions. E.g., selecting an appropriate sound fx.")]
        public FsmFloat relativeSpeed;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the world position of the collision contact. Useful for spawning effects etc.")]
        public FsmVector3 contactPoint;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the collision normal vector. Useful for aligning spawned effects etc.")]
        public FsmVector3 contactNormal;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The number of separate shaped regions in the collider.")]
        public FsmInt shapeCount;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the name of the physics 2D material of the colliding GameObject. Useful for triggering different effects. Audio, particles...")]
        public FsmString physics2dMaterialName;

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
            this.shapeCount = null;
            this.physics2dMaterialName = null;
        }

        private void StoreCollisionInfo()
        {
            if (base.Fsm.get_Collision2DInfo() != null)
            {
                this.gameObjectHit.set_Value(base.Fsm.get_Collision2DInfo().gameObject);
                this.relativeSpeed.Value = base.Fsm.get_Collision2DInfo().relativeVelocity.magnitude;
                this.relativeVelocity.set_Value((Vector3) base.Fsm.get_Collision2DInfo().relativeVelocity);
                this.physics2dMaterialName.Value = (base.Fsm.get_Collision2DInfo().collider.sharedMaterial != null) ? base.Fsm.get_Collision2DInfo().collider.sharedMaterial.name : "";
                this.shapeCount.Value = base.Fsm.get_Collision2DInfo().collider.shapeCount;
                if ((base.Fsm.get_Collision2DInfo().contacts != null) && (base.Fsm.get_Collision2DInfo().contacts.Length != 0))
                {
                    this.contactPoint.set_Value((Vector3) base.Fsm.get_Collision2DInfo().contacts[0].point);
                    this.contactNormal.set_Value((Vector3) base.Fsm.get_Collision2DInfo().contacts[0].normal);
                }
            }
        }
    }
}

