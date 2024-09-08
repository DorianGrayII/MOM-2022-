namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("RectTransforms position from world space into screen space. Leave the camera to none for default behavior")]
    public class RectTransformWorldToScreenPoint : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [CheckForComponent(typeof(Camera)), HutongGames.PlayMaker.Tooltip("The camera to perform the calculation. Leave to none for default behavior")]
        public FsmOwnerDefault camera;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the screen position in a Vector3 Variable. Z will equal zero.")]
        public FsmVector3 screenPoint;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the screen X position in a Float Variable.")]
        public FsmFloat screenX;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the screen Y position in a Float Variable.")]
        public FsmFloat screenY;
        [HutongGames.PlayMaker.Tooltip("Normalize screen coordinates (0-1). Otherwise coordinates are in pixels.")]
        public FsmBool normalize;
        private RectTransform _rt;
        private Camera _cam;

        private unsafe void DoWorldToScreenPoint()
        {
            Vector2 vector = RectTransformUtility.WorldToScreenPoint(this._cam, this._rt.position);
            if (this.normalize.Value)
            {
                float* singlePtr1 = &vector.x;
                singlePtr1[0] /= (float) Screen.width;
                float* singlePtr2 = &vector.y;
                singlePtr2[0] /= (float) Screen.height;
            }
            this.screenPoint.set_Value((Vector3) vector);
            this.screenX.Value = vector.x;
            this.screenY.Value = vector.y;
        }

        public override void OnActionUpdate()
        {
            this.DoWorldToScreenPoint();
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

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.camera = new FsmOwnerDefault();
            this.camera.OwnerOption = OwnerDefaultOption.SpecifyGameObject;
            FsmGameObject obj1 = new FsmGameObject();
            obj1.UseVariable = true;
            this.camera.GameObject = obj1;
            this.screenPoint = null;
            this.screenX = null;
            this.screenY = null;
            base.everyFrame = false;
        }
    }
}

