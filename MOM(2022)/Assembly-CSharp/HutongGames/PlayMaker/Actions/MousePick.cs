using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Input)]
    [Tooltip("Perform a Mouse Pick on the scene from the Main Camera and stores the results. Use Ray Distance to set how close the camera must be to pick the object.")]
    public class MousePick : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Set the length of the ray to cast from the Main Camera.")]
        public FsmFloat rayDistance = 100f;

        [UIHint(UIHint.Variable)]
        [Tooltip("Set Bool variable true if an object was picked, false if not.")]
        public FsmBool storeDidPickObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the picked GameObject.")]
        public FsmGameObject storeGameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the point of contact.")]
        public FsmVector3 storePoint;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the normal at the point of contact.")]
        public FsmVector3 storeNormal;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the distance to the point of contact.")]
        public FsmFloat storeDistance;

        [UIHint(UIHint.Layer)]
        [Tooltip("Pick only from these layers.")]
        public FsmInt[] layerMask;

        [Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
        public FsmBool invertMask;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.rayDistance = 100f;
            this.storeDidPickObject = null;
            this.storeGameObject = null;
            this.storePoint = null;
            this.storeNormal = null;
            this.storeDistance = null;
            this.layerMask = new FsmInt[0];
            this.invertMask = false;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoMousePick();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoMousePick();
        }

        private void DoMousePick()
        {
            RaycastHit raycastHit = ActionHelpers.MousePick(this.rayDistance.Value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
            bool flag = raycastHit.collider != null;
            this.storeDidPickObject.Value = flag;
            if (flag)
            {
                this.storeGameObject.Value = raycastHit.collider.gameObject;
                this.storeDistance.Value = raycastHit.distance;
                this.storePoint.Value = raycastHit.point;
                this.storeNormal.Value = raycastHit.normal;
            }
            else
            {
                this.storeGameObject.Value = null;
                this.storeDistance.Value = float.PositiveInfinity;
                this.storePoint.Value = Vector3.zero;
                this.storeNormal.Value = Vector3.zero;
            }
        }
    }
}
