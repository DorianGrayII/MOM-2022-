using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("Get the Local position of this RectTransform. This is Screen Space values using the anchoring as reference, so 0,0 is the center of the screen if the anchor is te center of the screen.")]
    public class RectTransformGetLocalPosition : BaseUpdateAction
    {
        public enum LocalPositionReference
        {
            Anchor = 0,
            CenterPosition = 1
        }

        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        public LocalPositionReference reference;

        [Tooltip("The position")]
        [UIHint(UIHint.Variable)]
        public FsmVector3 position;

        [Tooltip("The position in a Vector 2d ")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 position2d;

        [Tooltip("The x component of the Position")]
        [UIHint(UIHint.Variable)]
        public FsmFloat x;

        [Tooltip("The y component of the Position")]
        [UIHint(UIHint.Variable)]
        public FsmFloat y;

        [Tooltip("The z component of the Position")]
        [UIHint(UIHint.Variable)]
        public FsmFloat z;

        private RectTransform _rt;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.reference = LocalPositionReference.Anchor;
            this.position = null;
            this.position2d = null;
            this.x = null;
            this.y = null;
            this.z = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
            }
            this.DoGetValues();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.DoGetValues();
        }

        private void DoGetValues()
        {
            if (!(this._rt == null))
            {
                Vector3 localPosition = this._rt.localPosition;
                if (this.reference == LocalPositionReference.CenterPosition)
                {
                    localPosition.x += this._rt.rect.center.x;
                    localPosition.y += this._rt.rect.center.y;
                }
                if (!this.position.IsNone)
                {
                    this.position.Value = localPosition;
                }
                if (!this.position2d.IsNone)
                {
                    this.position2d.Value = new Vector2(localPosition.x, localPosition.y);
                }
                if (!this.x.IsNone)
                {
                    this.x.Value = localPosition.x;
                }
                if (!this.y.IsNone)
                {
                    this.y.Value = localPosition.y;
                }
                if (!this.z.IsNone)
                {
                    this.z.Value = localPosition.z;
                }
            }
        }
    }
}
