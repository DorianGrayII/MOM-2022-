using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics)]
    [Tooltip("Applies a force to a Game Object that simulates explosion effects. The explosion force will fall off linearly with distance. Hint: Use the Explosion Action instead to apply an explosion force to all objects in a blast radius.")]
    public class AddExplosionForce : ComponentAction<Rigidbody>
    {
        [RequiredField]
        [CheckForComponent(typeof(Rigidbody))]
        [Tooltip("The GameObject to add the explosion force to.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The center of the explosion. Hint: this is often the position returned from a GetCollisionInfo action.")]
        public FsmVector3 center;

        [RequiredField]
        [Tooltip("The strength of the explosion.")]
        public FsmFloat force;

        [RequiredField]
        [Tooltip("The radius of the explosion. Force falls off linearly with distance.")]
        public FsmFloat radius;

        [Tooltip("Applies the force as if it was applied from beneath the object. This is useful since explosions that throw things up instead of pushing things to the side look cooler. A value of 2 will apply a force as if it is applied from 2 meters below while not changing the actual explosion position.")]
        public FsmFloat upwardsModifier;

        [Tooltip("The type of force to apply. See Unity Physics docs.")]
        public ForceMode forceMode;

        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = null;
            this.center = new FsmVector3
            {
                UseVariable = true
            };
            this.upwardsModifier = 0f;
            this.forceMode = ForceMode.Force;
            this.everyFrame = false;
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleFixedUpdate = true;
        }

        public override void OnEnter()
        {
            this.DoAddExplosionForce();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            this.DoAddExplosionForce();
        }

        private void DoAddExplosionForce()
        {
            GameObject go = ((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
            if (this.center != null && base.UpdateCache(go))
            {
                base.rigidbody.AddExplosionForce(this.force.Value, this.center.Value, this.radius.Value, this.upwardsModifier.Value, this.forceMode);
            }
        }
    }
}
