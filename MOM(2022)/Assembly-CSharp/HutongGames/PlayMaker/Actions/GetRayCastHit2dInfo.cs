using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Gets info on the last 2d Raycast or LineCast and store in variables.")]
    public class GetRayCastHit2dInfo : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Get the GameObject hit by the last Raycast and store it in a variable.")]
        public FsmGameObject gameObjectHit;

        [UIHint(UIHint.Variable)]
        [Tooltip("Get the world position of the ray hit point and store it in a variable.")]
        [Title("Hit Point")]
        public FsmVector2 point;

        [UIHint(UIHint.Variable)]
        [Tooltip("Get the normal at the hit point and store it in a variable.")]
        public FsmVector3 normal;

        [UIHint(UIHint.Variable)]
        [Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
        public FsmFloat distance;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObjectHit = null;
            this.point = null;
            this.normal = null;
            this.distance = null;
            this.everyFrame = false;
        }

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

        private void StoreRaycastInfo()
        {
            RaycastHit2D lastRaycastHit2DInfo = Fsm.GetLastRaycastHit2DInfo(base.Fsm);
            if (lastRaycastHit2DInfo.collider != null)
            {
                this.gameObjectHit.Value = lastRaycastHit2DInfo.collider.gameObject;
                this.point.Value = lastRaycastHit2DInfo.point;
                this.normal.Value = lastRaycastHit2DInfo.normal;
                this.distance.Value = lastRaycastHit2DInfo.fraction;
            }
        }
    }
}
