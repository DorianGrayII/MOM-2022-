using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Input)]
    [Tooltip("Perform a raycast into the scene using screen coordinates and stores the results. Use Ray Distance to set how close the camera must be to pick the object. NOTE: Uses the MainCamera!")]
    public class ScreenPick : FsmStateAction
    {
        [Tooltip("A Vector3 screen position. Commonly stored by other actions.")]
        public FsmVector3 screenVector;

        [Tooltip("X position on screen.")]
        public FsmFloat screenX;

        [Tooltip("Y position on screen.")]
        public FsmFloat screenY;

        [Tooltip("Are the supplied screen coordinates normalized (0-1), or in pixels.")]
        public FsmBool normalized;

        [RequiredField]
        public FsmFloat rayDistance = 100f;

        [UIHint(UIHint.Variable)]
        public FsmBool storeDidPickObject;

        [UIHint(UIHint.Variable)]
        public FsmGameObject storeGameObject;

        [UIHint(UIHint.Variable)]
        public FsmVector3 storePoint;

        [UIHint(UIHint.Variable)]
        public FsmVector3 storeNormal;

        [UIHint(UIHint.Variable)]
        public FsmFloat storeDistance;

        [UIHint(UIHint.Layer)]
        [Tooltip("Pick only from these layers.")]
        public FsmInt[] layerMask;

        [Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
        public FsmBool invertMask;

        public bool everyFrame;

        public override void Reset()
        {
            this.screenVector = new FsmVector3
            {
                UseVariable = true
            };
            this.screenX = new FsmFloat
            {
                UseVariable = true
            };
            this.screenY = new FsmFloat
            {
                UseVariable = true
            };
            this.normalized = false;
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

        private void DoScreenPick()
        {
            if (Camera.main == null)
            {
                base.LogError("No MainCamera defined!");
                base.Finish();
                return;
            }
            Vector3 pos = Vector3.zero;
            if (!this.screenVector.IsNone)
            {
                pos = this.screenVector.Value;
            }
            if (!this.screenX.IsNone)
            {
                pos.x = this.screenX.Value;
            }
            if (!this.screenY.IsNone)
            {
                pos.y = this.screenY.Value;
            }
            if (this.normalized.Value)
            {
                pos.x *= Screen.width;
                pos.y *= Screen.height;
            }
            Physics.Raycast(Camera.main.ScreenPointToRay(pos), out var hitInfo, this.rayDistance.Value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
            bool flag = hitInfo.collider != null;
            this.storeDidPickObject.Value = flag;
            if (flag)
            {
                this.storeGameObject.Value = hitInfo.collider.gameObject;
                this.storeDistance.Value = hitInfo.distance;
                this.storePoint.Value = hitInfo.point;
                this.storeNormal.Value = hitInfo.normal;
            }
            else
            {
                this.storeGameObject.Value = null;
                this.storeDistance = float.PositiveInfinity;
                this.storePoint.Value = Vector3.zero;
                this.storeNormal.Value = Vector3.zero;
            }
        }
    }
}
