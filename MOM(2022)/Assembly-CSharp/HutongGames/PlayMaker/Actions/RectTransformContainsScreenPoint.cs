namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("Check if a RectTransform contains the screen point as seen from the given camera.")]
    public class RectTransformContainsScreenPoint : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The screenPoint as a Vector2. Leave to none if you want to use the Vector3 alternative")]
        public FsmVector2 screenPointVector2;
        [HutongGames.PlayMaker.Tooltip("The screenPoint as a Vector3. Leave to none if you want to use the Vector2 alternative")]
        public FsmVector3 orScreenPointVector3;
        [HutongGames.PlayMaker.Tooltip("Define if screenPoint are expressed as normalized screen coordinates (0-1). Otherwise coordinates are in pixels.")]
        public bool normalizedScreenPoint;
        [HutongGames.PlayMaker.Tooltip("The Camera. For a RectTransform in a Canvas set to Screen Space - Overlay mode, the cam parameter should be set to null explicitly (default).\nLeave to none and the camera will be the one from EventSystem.current.camera"), CheckForComponent(typeof(Camera))]
        public FsmGameObject camera;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
        public bool everyFrame;
        [ActionSection("Result"), HutongGames.PlayMaker.Tooltip("Store the result."), UIHint(UIHint.Variable)]
        public FsmBool isContained;
        [HutongGames.PlayMaker.Tooltip("Event sent if screenPoint is contained in RectTransform.")]
        public FsmEvent isContainedEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if screenPoint is NOT contained in RectTransform.")]
        public FsmEvent isNotContainedEvent;
        private RectTransform _rt;
        private Camera _camera;

        private unsafe void DoCheck()
        {
            if (this._rt != null)
            {
                Vector2 screenPoint = this.screenPointVector2.get_Value();
                if (!this.orScreenPointVector3.IsNone)
                {
                    screenPoint.x = this.orScreenPointVector3.get_Value().x;
                    screenPoint.y = this.orScreenPointVector3.get_Value().y;
                }
                if (this.normalizedScreenPoint)
                {
                    float* singlePtr1 = &screenPoint.x;
                    singlePtr1[0] *= Screen.width;
                    float* singlePtr2 = &screenPoint.y;
                    singlePtr2[0] *= Screen.height;
                }
                bool flag = RectTransformUtility.RectangleContainsScreenPoint(this._rt, screenPoint, this._camera);
                if (!this.isContained.IsNone)
                {
                    this.isContained.Value = flag;
                }
                if (!flag)
                {
                    if (this.isNotContainedEvent != null)
                    {
                        base.Fsm.Event(this.isNotContainedEvent);
                    }
                }
                else if (this.isContainedEvent != null)
                {
                    base.Fsm.Event(this.isContainedEvent);
                }
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
            }
            this._camera = this.camera.IsNone ? EventSystem.current.GetComponent<Camera>() : this.camera.get_Value().GetComponent<Camera>();
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

        public override void Reset()
        {
            this.gameObject = null;
            this.screenPointVector2 = null;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.orScreenPointVector3 = vector1;
            this.normalizedScreenPoint = false;
            this.camera = null;
            this.everyFrame = false;
            this.isContained = null;
            this.isContainedEvent = null;
            this.isNotContainedEvent = null;
        }
    }
}

