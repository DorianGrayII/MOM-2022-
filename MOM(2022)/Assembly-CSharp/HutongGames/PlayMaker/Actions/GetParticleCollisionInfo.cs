namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics)]
    [Tooltip("Gets info on the last particle collision event. See Unity Particle System docs.")]
    public class GetParticleCollisionInfo : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Get the GameObject hit.")]
        public FsmGameObject gameObjectHit;

        public override void Reset()
        {
            this.gameObjectHit = null;
        }

        private void StoreCollisionInfo()
        {
            this.gameObjectHit.Value = base.Fsm.ParticleCollisionGO;
        }

        public override void OnEnter()
        {
            this.StoreCollisionInfo();
            base.Finish();
        }
    }
}
