using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Camera)]
    [Tooltip("Transforms position from world space into screen space. NOTE: Uses the MainCamera!")]
    public class WorldToScreenPoint : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("World position to transform into screen coordinates.")]
        public FsmVector3 worldPosition;

        [Tooltip("World X position.")]
        public FsmFloat worldX;

        [Tooltip("World Y position.")]
        public FsmFloat worldY;

        [Tooltip("World Z position.")]
        public FsmFloat worldZ;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the screen position in a Vector3 Variable. Z will equal zero.")]
        public FsmVector3 storeScreenPoint;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the screen X position in a Float Variable.")]
        public FsmFloat storeScreenX;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the screen Y position in a Float Variable.")]
        public FsmFloat storeScreenY;

        [Tooltip("Normalize screen coordinates (0-1). Otherwise coordinates are in pixels.")]
        public FsmBool normalize;

        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        public override void Reset()
        {
            this.worldPosition = null;
            this.worldX = new FsmFloat
            {
                UseVariable = true
            };
            this.worldY = new FsmFloat
            {
                UseVariable = true
            };
            this.worldZ = new FsmFloat
            {
                UseVariable = true
            };
            this.storeScreenPoint = null;
            this.storeScreenX = null;
            this.storeScreenY = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoWorldToScreenPoint();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoWorldToScreenPoint();
        }

        private void DoWorldToScreenPoint()
        {
            if (Camera.main == null)
            {
                base.LogError("No MainCamera defined!");
                base.Finish();
                return;
            }
            Vector3 position = Vector3.zero;
            if (!this.worldPosition.IsNone)
            {
                position = this.worldPosition.Value;
            }
            if (!this.worldX.IsNone)
            {
                position.x = this.worldX.Value;
            }
            if (!this.worldY.IsNone)
            {
                position.y = this.worldY.Value;
            }
            if (!this.worldZ.IsNone)
            {
                position.z = this.worldZ.Value;
            }
            position = Camera.main.WorldToScreenPoint(position);
            if (this.normalize.Value)
            {
                position.x /= Screen.width;
                position.y /= Screen.height;
            }
            this.storeScreenPoint.Value = position;
            this.storeScreenX.Value = position.x;
            this.storeScreenY.Value = position.y;
        }
    }
}
