namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Physics2D), Tooltip("Gets info on the last Trigger 2d event and store in variables.  See Unity and PlayMaker docs on Unity 2D physics.")]
    public class GetTrigger2dInfo : FsmStateAction
    {
        [UIHint(UIHint.Variable), Tooltip("Get the GameObject hit.")]
        public FsmGameObject gameObjectHit;
        [UIHint(UIHint.Variable), Tooltip("The number of separate shaped regions in the collider.")]
        public FsmInt shapeCount;
        [UIHint(UIHint.Variable), Tooltip("Useful for triggering different effects. Audio, particles...")]
        public FsmString physics2dMaterialName;

        public override void OnEnter()
        {
            this.StoreTriggerInfo();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObjectHit = null;
            this.shapeCount = null;
            this.physics2dMaterialName = null;
        }

        private void StoreTriggerInfo()
        {
            if (base.Fsm.get_TriggerCollider2D() != null)
            {
                this.gameObjectHit.set_Value(base.Fsm.get_TriggerCollider2D().gameObject);
                this.shapeCount.Value = base.Fsm.get_TriggerCollider2D().shapeCount;
                this.physics2dMaterialName.Value = (base.Fsm.get_TriggerCollider2D().sharedMaterial != null) ? base.Fsm.get_TriggerCollider2D().sharedMaterial.name : "";
            }
        }
    }
}

