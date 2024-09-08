namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Gets info on the last 2d Raycast or LineCast and store in variables.")]
    public class GetRayCastHit2dInfo : FsmStateAction
    {
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the GameObject hit by the last Raycast and store it in a variable.")]
        public FsmGameObject gameObjectHit;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the world position of the ray hit point and store it in a variable."), Title("Hit Point")]
        public FsmVector2 point;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the normal at the hit point and store it in a variable.")]
        public FsmVector3 normal;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
        public FsmFloat distance;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
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
            RaycastHit2D hitd = Fsm.GetLastRaycastHit2DInfo(base.Fsm);
            if (hitd.collider != null)
            {
                this.gameObjectHit.set_Value(hitd.collider.gameObject);
                this.point.set_Value(hitd.point);
                this.normal.set_Value((Vector3) hitd.normal);
                this.distance.Value = hitd.fraction;
            }
        }
    }
}

