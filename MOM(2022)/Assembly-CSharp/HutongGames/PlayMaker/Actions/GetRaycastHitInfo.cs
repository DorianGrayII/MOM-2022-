namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Physics), Tooltip("Gets info on the last Raycast and store in variables.")]
    public class GetRaycastHitInfo : FsmStateAction
    {
        [UIHint(UIHint.Variable), Tooltip("Get the GameObject hit by the last Raycast and store it in a variable.")]
        public FsmGameObject gameObjectHit;
        [UIHint(UIHint.Variable), Tooltip("Get the world position of the ray hit point and store it in a variable."), Title("Hit Point")]
        public FsmVector3 point;
        [UIHint(UIHint.Variable), Tooltip("Get the normal at the hit point and store it in a variable.")]
        public FsmVector3 normal;
        [UIHint(UIHint.Variable), Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
        public FsmFloat distance;
        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void OnEnter()
        {
            this.StoreRaycastInfo();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.StoreRaycastInfo();
        }

        public override void Reset()
        {
            this.gameObjectHit = null;
            this.point = null;
            this.normal = null;
            this.distance = null;
            this.everyFrame = false;
        }

        private void StoreRaycastInfo()
        {
            if (base.Fsm.get_RaycastHitInfo().collider != null)
            {
                this.gameObjectHit.set_Value(base.Fsm.get_RaycastHitInfo().collider.gameObject);
                this.point.set_Value(base.Fsm.get_RaycastHitInfo().point);
                this.normal.set_Value(base.Fsm.get_RaycastHitInfo().normal);
                this.distance.Value = base.Fsm.get_RaycastHitInfo().distance;
            }
        }
    }
}

