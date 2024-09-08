namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Perform a Mouse Pick on a 2d scene and stores the results. Use Ray Distance to set how close the camera must be to pick the 2d object.")]
    public class MousePick2d : FsmStateAction
    {
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store if a GameObject was picked in a Bool variable. True if a GameObject was picked, otherwise false.")]
        public FsmBool storeDidPickObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the picked GameObject in a variable.")]
        public FsmGameObject storeGameObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the picked point in a variable.")]
        public FsmVector2 storePoint;
        [UIHint(UIHint.Layer), HutongGames.PlayMaker.Tooltip("Pick only from these layers.")]
        public FsmInt[] layerMask;
        [HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
        public FsmBool invertMask;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoMousePick2d()
        {
            RaycastHit2D hitd = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), float.PositiveInfinity, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
            bool flag = hitd.collider != null;
            this.storeDidPickObject.Value = flag;
            if (flag)
            {
                this.storeGameObject.set_Value(hitd.collider.gameObject);
                this.storePoint.set_Value(hitd.point);
            }
            else
            {
                this.storeGameObject.set_Value((GameObject) null);
                this.storePoint.set_Value(Vector3.zero);
            }
        }

        public override void OnEnter()
        {
            this.DoMousePick2d();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoMousePick2d();
        }

        public override void Reset()
        {
            this.storeDidPickObject = null;
            this.storeGameObject = null;
            this.storePoint = null;
            this.layerMask = new FsmInt[0];
            this.invertMask = false;
            this.everyFrame = false;
        }
    }
}

