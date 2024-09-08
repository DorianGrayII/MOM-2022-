using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("Given a rect transform, return the corner points in pixel accurate coordinates.")]
    public class RectTransformPixelAdjustRect : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [CheckForComponent(typeof(Canvas))]
        [Tooltip("The canvas. Leave to none to use the canvas of the gameObject")]
        public FsmGameObject canvas;

        [ActionSection("Result")]
        [RequiredField]
        [Tooltip("Pixel adjusted rect.")]
        [UIHint(UIHint.Variable)]
        public FsmRect pixelRect;

        private RectTransform _rt;

        private Canvas _canvas;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.canvas = new FsmGameObject
            {
                UseVariable = true
            };
            this.pixelRect = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
            }
            GameObject value = this.canvas.Value;
            if (value != null)
            {
                this._canvas = value.GetComponent<Canvas>();
            }
            if (this._canvas == null && ownerDefaultTarget != null)
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

        public override void OnActionUpdate()
        {
            this.DoAction();
        }

        private void DoAction()
        {
            this.pixelRect.Value = RectTransformUtility.PixelAdjustRect(this._rt, this._canvas);
        }
    }
}
