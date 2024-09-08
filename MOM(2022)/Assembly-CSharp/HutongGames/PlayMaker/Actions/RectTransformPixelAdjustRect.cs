namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("Given a rect transform, return the corner points in pixel accurate coordinates.")]
    public class RectTransformPixelAdjustRect : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, CheckForComponent(typeof(Canvas)), HutongGames.PlayMaker.Tooltip("The canvas. Leave to none to use the canvas of the gameObject")]
        public FsmGameObject canvas;
        [ActionSection("Result"), RequiredField, HutongGames.PlayMaker.Tooltip("Pixel adjusted rect."), UIHint(UIHint.Variable)]
        public FsmRect pixelRect;
        private RectTransform _rt;
        private Canvas _canvas;

        private void DoAction()
        {
            this.pixelRect.set_Value(RectTransformUtility.PixelAdjustRect(this._rt, this._canvas));
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
            this.pixelRect = null;
        }
    }
}

