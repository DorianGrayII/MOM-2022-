using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("The position ( normalized or not) in the parent RectTransform keeping the anchor rect size intact. This lets you position the whole Rect in one go. Use this to easily animate movement (like IOS sliding UIView)")]
    public class RectTransformSetAnchorRectPosition : BaseUpdateAction
    {
        public enum AnchorReference
        {
            TopLeft = 0,
            Top = 1,
            TopRight = 2,
            Right = 3,
            BottomRight = 4,
            Bottom = 5,
            BottomLeft = 6,
            Left = 7,
            Center = 8
        }

        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The reference for the given position")]
        public AnchorReference anchorReference;

        [Tooltip("Are the supplied screen coordinates normalized (0-1), or in pixels.")]
        public FsmBool normalized;

        [Tooltip("The Vector2 position, and/or set individual axis below.")]
        public FsmVector2 anchor;

        [HasFloatSlider(0f, 1f)]
        public FsmFloat x;

        [HasFloatSlider(0f, 1f)]
        public FsmFloat y;

        private RectTransform _rt;

        private Rect _anchorRect;

        public override void Reset()
        {
            base.Reset();
            this.normalized = true;
            this.gameObject = null;
            this.anchorReference = AnchorReference.BottomLeft;
            this.anchor = null;
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
            this.DoSetAnchor();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.DoSetAnchor();
        }

        private void DoSetAnchor()
        {
            this._anchorRect = default(Rect);
            this._anchorRect.min = this._rt.anchorMin;
            this._anchorRect.max = this._rt.anchorMax;
            Vector2 zero = Vector2.zero;
            zero = this._anchorRect.min;
            if (!this.anchor.IsNone)
            {
                if (this.normalized.Value)
                {
                    zero = this.anchor.Value;
                }
                else
                {
                    zero.x = this.anchor.Value.x / (float)Screen.width;
                    zero.y = this.anchor.Value.y / (float)Screen.height;
                }
            }
            if (!this.x.IsNone)
            {
                if (this.normalized.Value)
                {
                    zero.x = this.x.Value;
                }
                else
                {
                    zero.x = this.x.Value / (float)Screen.width;
                }
            }
            if (!this.y.IsNone)
            {
                if (this.normalized.Value)
                {
                    zero.y = this.y.Value;
                }
                else
                {
                    zero.y = this.y.Value / (float)Screen.height;
                }
            }
            if (this.anchorReference == AnchorReference.BottomLeft)
            {
                this._anchorRect.x = zero.x;
                this._anchorRect.y = zero.y;
            }
            else if (this.anchorReference == AnchorReference.Left)
            {
                this._anchorRect.x = zero.x;
                this._anchorRect.y = zero.y - 0.5f;
            }
            else if (this.anchorReference == AnchorReference.TopLeft)
            {
                this._anchorRect.x = zero.x;
                this._anchorRect.y = zero.y - 1f;
            }
            else if (this.anchorReference == AnchorReference.Top)
            {
                this._anchorRect.x = zero.x - 0.5f;
                this._anchorRect.y = zero.y - 1f;
            }
            else if (this.anchorReference == AnchorReference.TopRight)
            {
                this._anchorRect.x = zero.x - 1f;
                this._anchorRect.y = zero.y - 1f;
            }
            else if (this.anchorReference == AnchorReference.Right)
            {
                this._anchorRect.x = zero.x - 1f;
                this._anchorRect.y = zero.y - 0.5f;
            }
            else if (this.anchorReference == AnchorReference.BottomRight)
            {
                this._anchorRect.x = zero.x - 1f;
                this._anchorRect.y = zero.y;
            }
            else if (this.anchorReference == AnchorReference.Bottom)
            {
                this._anchorRect.x = zero.x - 0.5f;
                this._anchorRect.y = zero.y;
            }
            else if (this.anchorReference == AnchorReference.Center)
            {
                this._anchorRect.x = zero.x - 0.5f;
                this._anchorRect.y = zero.y - 0.5f;
            }
            this._rt.anchorMin = this._anchorRect.min;
            this._rt.anchorMax = this._anchorRect.max;
        }
    }
}
