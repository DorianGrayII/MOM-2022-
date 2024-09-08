namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("Convert a given point in screen space into a pixel correct point.")]
    public class RectTransformPixelAdjustPoint : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, CheckForComponent(typeof(Canvas)), HutongGames.PlayMaker.Tooltip("The canvas. Leave to none to use the canvas of the gameObject")]
        public FsmGameObject canvas;
        [HutongGames.PlayMaker.Tooltip("The screen position.")]
        public FsmVector2 screenPoint;
        [ActionSection("Result"), RequiredField, HutongGames.PlayMaker.Tooltip("Pixel adjusted point from the screen position."), UIHint(UIHint.Variable)]
        public FsmVector2 pixelPoint;
        private RectTransform _rt;
        private Canvas _canvas;

        private void DoAction()
        {
            this.pixelPoint.set_Value(RectTransformUtility.PixelAdjustPoint(this.screenPoint.get_Value(), this._rt, this._canvas));
        }

        public override void OnActionUpdate()
        {
            this.DoAction();
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
            }
            GameObject obj3 = this.canvas.get_Value();
            if (obj3 != null)
            {
                this._canvas = obj3.GetComponent<Canvas>();
            }
            if ((this._canvas == null) && (ownerDefaultTarget != null))
            {
                Graphic component = ownerDefaultTarget.GetComponent<Graphic>();
                if (component != null)
                {
                    this._canvas = component.canvas;
                }
            }
            this.DoAction();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            FsmGameObject obj1 = new FsmGameObject();
            obj1.UseVariable = true;
            this.canvas = obj1;
            this.screenPoint = null;
            this.pixelPoint = null;
        }
    }
}

