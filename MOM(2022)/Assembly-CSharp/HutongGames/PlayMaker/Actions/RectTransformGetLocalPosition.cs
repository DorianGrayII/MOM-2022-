namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("Get the Local position of this RectTransform. This is Screen Space values using the anchoring as reference, so 0,0 is the center of the screen if the anchor is te center of the screen.")]
    public class RectTransformGetLocalPosition : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        public LocalPositionReference reference;
        [HutongGames.PlayMaker.Tooltip("The position"), UIHint(UIHint.Variable)]
        public FsmVector3 position;
        [HutongGames.PlayMaker.Tooltip("The position in a Vector 2d "), UIHint(UIHint.Variable)]
        public FsmVector2 position2d;
        [HutongGames.PlayMaker.Tooltip("The x component of the Position"), UIHint(UIHint.Variable)]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("The y component of the Position"), UIHint(UIHint.Variable)]
        public FsmFloat y;
        [HutongGames.PlayMaker.Tooltip("The z component of the Position"), UIHint(UIHint.Variable)]
        public FsmFloat z;
        private RectTransform _rt;

        private unsafe void DoGetValues()
        {
            if (this._rt != null)
            {
                Vector3 localPosition = this._rt.localPosition;
                if (this.reference == LocalPositionReference.CenterPosition)
                {
                    float* singlePtr1 = &localPosition.x;
                    singlePtr1[0] += this._rt.rect.center.x;
                    float* singlePtr2 = &localPosition.y;
                    singlePtr2[0] += this._rt.rect.center.y;
                }
                if (!this.position.IsNone)
                {
                    this.position.set_Value(localPosition);
                }
                if (!this.position2d.IsNone)
                {
                    this.position2d.set_Value(new Vector2(localPosition.x, localPosition.y));
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

        public override void OnActionUpdate()
        {
            this.DoGetValues();
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

        public enum LocalPositionReference
        {
            Anchor,
            CenterPosition
        }
    }
}

