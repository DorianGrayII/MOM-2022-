namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Perform a raycast into the 2d scene using screen coordinates and stores the results. Use Ray Distance to set how close the camera must be to pick the 2d object. NOTE: Uses the MainCamera!")]
    public class ScreenPick2d : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("A Vector3 screen position. Commonly stored by other actions.")]
        public FsmVector3 screenVector;
        [HutongGames.PlayMaker.Tooltip("X position on screen.")]
        public FsmFloat screenX;
        [HutongGames.PlayMaker.Tooltip("Y position on screen.")]
        public FsmFloat screenY;
        [HutongGames.PlayMaker.Tooltip("Are the supplied screen coordinates normalized (0-1), or in pixels.")]
        public FsmBool normalized;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store whether the Screen pick did pick a GameObject")]
        public FsmBool storeDidPickObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the picked GameObject")]
        public FsmGameObject storeGameObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the picked position in world Space")]
        public FsmVector3 storePoint;
        [UIHint(UIHint.Layer), HutongGames.PlayMaker.Tooltip("Pick only from these layers.")]
        public FsmInt[] layerMask;
        [HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
        public FsmBool invertMask;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private unsafe void DoScreenPick()
        {
            if (Camera.main == null)
            {
                base.LogError("No MainCamera defined!");
                base.Finish();
            }
            else
            {
                Vector3 zero = Vector3.zero;
                if (!this.screenVector.IsNone)
                {
                    zero = this.screenVector.get_Value();
                }
                if (!this.screenX.IsNone)
                {
                    zero.x = this.screenX.Value;
                }
                if (!this.screenY.IsNone)
                {
                    zero.y = this.screenY.Value;
                }
                if (this.normalized.Value)
                {
                    float* singlePtr1 = &zero.x;
                    singlePtr1[0] *= Screen.width;
                    float* singlePtr2 = &zero.y;
                    singlePtr2[0] *= Screen.height;
                }
                RaycastHit2D hitd = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(zero), float.PositiveInfinity, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
                bool flag = hitd.collider != null;
                this.storeDidPickObject.Value = flag;
                if (flag)
                {
                    this.storeGameObject.set_Value(hitd.collider.gameObject);
                    this.storePoint.set_Value((Vector3) hitd.point);
                }
                else
                {
                    this.storeGameObject.set_Value((GameObject) null);
                    this.storePoint.set_Value(Vector3.zero);
                }
            }
        }

        public override void OnEnter()
        {
            this.DoScreenPick();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoScreenPick();
        }

        public override void Reset()
        {
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.screenVector = vector1;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.screenX = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.screenY = num2;
            this.normalized = false;
            this.storeDidPickObject = null;
            this.storeGameObject = null;
            this.storePoint = null;
            this.layerMask = new FsmInt[0];
            this.invertMask = false;
            this.everyFrame = false;
        }
    }
}

