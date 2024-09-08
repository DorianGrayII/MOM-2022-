using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("The position of the pivot of this RectTransform relative to the anchor reference point.The anchor reference point is where the anchors are. If the anchor are not together, the four anchor positions are interpolated according to the pivot normalized values.")]
    public class RectTransformSetAnchoredPosition : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The Vector2 position. Set to none for no effect, and/or set individual axis below. ")]
        public FsmVector2 position;

        [Tooltip("Setting only the x value. Overrides position x value if set. Set to none for no effect")]
        public FsmFloat x;

        [Tooltip("Setting only the y value. Overrides position x value if set. Set to none for no effect")]
        public FsmFloat y;

        private RectTransform _rt;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.position = null;
            this.x = new FsmFloat
            {
                UseVariable = true
            };
            this.y = new FsmFloat
            {
                UseVariable = true
            };
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
            }
            this.DoSetAnchoredPosition();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.DoSetAnchoredPosition();
        }

        private void DoSetAnchoredPosition()
        {
            Vector2 anchoredPosition = this._rt.anchoredPosition;
            if (!this.position.IsNone)
            {
                anchoredPosition = this.position.Value;
            }
            if (!this.x.IsNone)
            {
                anchoredPosition.x = this.x.Value;
            }
            if (!this.y.IsNone)
            {
                anchoredPosition.y = this.y.Value;
            }
            this._rt.anchoredPosition = anchoredPosition;
        }
    }
}
