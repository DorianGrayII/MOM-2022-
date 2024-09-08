using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("RectTransforms position from world space into screen space. Leave the camera to none for default behavior")]
    public class RectTransformWorldToScreenPoint : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [CheckForComponent(typeof(Camera))]
        [Tooltip("The camera to perform the calculation. Leave to none for default behavior")]
        public FsmOwnerDefault camera;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the screen position in a Vector3 Variable. Z will equal zero.")]
        public FsmVector3 screenPoint;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the screen X position in a Float Variable.")]
        public FsmFloat screenX;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the screen Y position in a Float Variable.")]
        public FsmFloat screenY;

        [Tooltip("Normalize screen coordinates (0-1). Otherwise coordinates are in pixels.")]
        public FsmBool normalize;

        private RectTransform _rt;

        private Camera _cam;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.camera = new FsmOwnerDefault();
            this.camera.OwnerOption = OwnerDefaultOption.SpecifyGameObject;
            this.camera.GameObject = new FsmGameObject
            {
                UseVariable = true
            };
            this.screenPoint = null;
            this.screenX = null;
            this.screenY = null;
            base.everyFrame = false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
            }
            if (base.Fsm.GetOwnerDefaultTarget(this.camera) != null)
            {
                this._cam = ownerDefaultTarget.GetComponent<Camera>();
            }
            this.DoWorldToScreenPoint();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.DoWorldToScreenPoint();
        }

        private void DoWorldToScreenPoint()
        {
            Vector2 vector = RectTransformUtility.WorldToScreenPoint(this._cam, this._rt.position);
            if (this.normalize.Value)
            {
                vector.x /= Screen.width;
                vector.y /= Screen.height;
            }
            this.screenPoint.Value = vector;
            this.screenX.Value = vector.x;
            this.screenY.Value = vector.y;
        }
    }
}
