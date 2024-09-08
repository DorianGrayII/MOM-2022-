using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("Check if a RectTransform contains the screen point as seen from the given camera.")]
    public class RectTransformContainsScreenPoint : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The screenPoint as a Vector2. Leave to none if you want to use the Vector3 alternative")]
        public FsmVector2 screenPointVector2;

        [Tooltip("The screenPoint as a Vector3. Leave to none if you want to use the Vector2 alternative")]
        public FsmVector3 orScreenPointVector3;

        [Tooltip("Define if screenPoint are expressed as normalized screen coordinates (0-1). Otherwise coordinates are in pixels.")]
        public bool normalizedScreenPoint;

        [Tooltip("The Camera. For a RectTransform in a Canvas set to Screen Space - Overlay mode, the cam parameter should be set to null explicitly (default).\nLeave to none and the camera will be the one from EventSystem.current.camera")]
        [CheckForComponent(typeof(Camera))]
        public FsmGameObject camera;

        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        [ActionSection("Result")]
        [Tooltip("Store the result.")]
        [UIHint(UIHint.Variable)]
        public FsmBool isContained;

        [Tooltip("Event sent if screenPoint is contained in RectTransform.")]
        public FsmEvent isContainedEvent;

        [Tooltip("Event sent if screenPoint is NOT contained in RectTransform.")]
        public FsmEvent isNotContainedEvent;

        private RectTransform _rt;

        private Camera _camera;

        public override void Reset()
        {
            this.gameObject = null;
            this.screenPointVector2 = null;
            this.orScreenPointVector3 = new FsmVector3
            {
                UseVariable = true
            };
            this.normalizedScreenPoint = false;
            this.camera = null;
            this.everyFrame = false;
            this.isContained = null;
            this.isContainedEvent = null;
            this.isNotContainedEvent = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
            }
            if (!this.camera.IsNone)
            {
                this._camera = this.camera.Value.GetComponent<Camera>();
            }
            else
            {
                this._camera = EventSystem.current.GetComponent<Camera>();
            }
            this.DoCheck();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoCheck();
        }

        private void DoCheck()
        {
            if (this._rt == null)
            {
                return;
            }
            Vector2 value = this.screenPointVector2.Value;
            if (!this.orScreenPointVector3.IsNone)
            {
                value.x = this.orScreenPointVector3.Value.x;
                value.y = this.orScreenPointVector3.Value.y;
            }
            if (this.normalizedScreenPoint)
            {
                value.x *= Screen.width;
                value.y *= Screen.height;
            }
            bool flag = RectTransformUtility.RectangleContainsScreenPoint(this._rt, value, this._camera);
            if (!this.isContained.IsNone)
            {
                this.isContained.Value = flag;
            }
            if (flag)
            {
                if (this.isContainedEvent != null)
                {
                    base.Fsm.Event(this.isContainedEvent);
                }
            }
            else if (this.isNotContainedEvent != null)
            {
                base.Fsm.Event(this.isNotContainedEvent);
            }
        }
    }
}
