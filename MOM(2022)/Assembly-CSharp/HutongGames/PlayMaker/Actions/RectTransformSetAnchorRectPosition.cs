﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("The position ( normalized or not) in the parent RectTransform keeping the anchor rect size intact. This lets you position the whole Rect in one go. Use this to easily animate movement (like IOS sliding UIView)")]
    public class RectTransformSetAnchorRectPosition : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The reference for the given position")]
        public AnchorReference anchorReference;
        [HutongGames.PlayMaker.Tooltip("Are the supplied screen coordinates normalized (0-1), or in pixels.")]
        public FsmBool normalized;
        [HutongGames.PlayMaker.Tooltip("The Vector2 position, and/or set individual axis below.")]
        public FsmVector2 anchor;
        [HasFloatSlider(0f, 1f)]
        public FsmFloat x;
        [HasFloatSlider(0f, 1f)]
        public FsmFloat y;
        private RectTransform _rt;
        private Rect _anchorRect;

        private void DoSetAnchor()
        {
            this._anchorRect = new Rect();
            this._anchorRect.min = this._rt.anchorMin;
            this._anchorRect.max = this._rt.anchorMax;
            Vector2 zero = Vector2.zero;
            zero = this._anchorRect.min;
            if (!this.anchor.IsNone)
            {
                if (this.normalized.Value)
                {
                    zero = this.anchor.get_Value();
                }
                else
                {
                    zero.x = this.anchor.get_Value().x / ((float) Screen.width);
                    zero.y = this.anchor.get_Value().y / ((float) Screen.height);
                }
            }
            if (!this.x.IsNone)
            {
                zero.x = !this.normalized.Value ? (this.x.Value / ((float) Screen.width)) : this.x.Value;
            }
            if (!this.y.IsNone)
            {
                zero.y = !this.normalized.Value ? (this.y.Value / ((float) Screen.height)) : this.y.Value;
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

        public override void OnActionUpdate()
        {
            this.DoSetAnchor();
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

        public override void Reset()
        {
            base.Reset();
            this.normalized = true;
            this.gameObject = null;
            this.anchorReference = AnchorReference.BottomLeft;
            this.anchor = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.x = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.y = num2;
        }

        public enum AnchorReference
        {
            TopLeft,
            Top,
            TopRight,
            Right,
            BottomRight,
            Bottom,
            BottomLeft,
            Left,
            Center
        }
    }
}

