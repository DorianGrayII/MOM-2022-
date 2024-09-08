namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Perform a Mouse Pick on the scene from the Main Camera and stores the results. Use Ray Distance to set how close the camera must be to pick the object.")]
    public class MousePick : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("Set the length of the ray to cast from the Main Camera.")]
        public FsmFloat rayDistance = 100f;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Set Bool variable true if an object was picked, false if not.")]
        public FsmBool storeDidPickObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the picked GameObject.")]
        public FsmGameObject storeGameObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the point of contact.")]
        public FsmVector3 storePoint;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the normal at the point of contact.")]
        public FsmVector3 storeNormal;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the distance to the point of contact.")]
        public FsmFloat storeDistance;
        [UIHint(UIHint.Layer), HutongGames.PlayMaker.Tooltip("Pick only from these layers.")]
        public FsmInt[] layerMask;
        [HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
        public FsmBool invertMask;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoMousePick()
        {
            RaycastHit hit = ActionHelpers.MousePick(this.rayDistance.Value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
            bool flag = hit.collider != null;
            this.storeDidPickObject.Value = flag;
            if (flag)
            {
                this.storeGameObject.set_Value(hit.collider.gameObject);
                this.storeDistance.Value = hit.distance;
                this.storePoint.set_Value(hit.point);
                this.storeNormal.set_Value(hit.normal);
            }
            else
            {
                this.storeGameObject.set_Value((GameObject) null);
                this.storeDistance.Value = float.PositiveInfinity;
                this.storePoint.set_Value(Vector3.zero);
                this.storeNormal.set_Value(Vector3.zero);
            }
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
    }
}

