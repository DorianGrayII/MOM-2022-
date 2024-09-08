using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("The normalized position in the parent RectTransform that the upper right corner is anchored to. This is relative screen space, values ranges from 0 to 1")]
    public class RectTransformGetAnchorMinAndMax : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The Vector2 anchor max. Set to none for no effect, and/or set individual axis below.")]
        public FsmVector2 anchorMax;

        [Tooltip("The Vector2 anchor min. Set to none for no effect, and/or set individual axis below.")]
        public FsmVector2 anchorMin;

        [HasFloatSlider(0f, 1f)]
        [Tooltip("Setting only the x value. Overrides anchorMax x value if set. Set to none for no effect")]
        public FsmFloat xMax;

        [HasFloatSlider(0f, 1f)]
        [Tooltip("Setting only the x value. Overrides anchorMax x value if set. Set to none for no effect")]
        public FsmFloat yMax;

        [HasFloatSlider(0f, 1f)]
        [Tooltip("Setting only the x value. Overrides anchorMin x value if set. Set to none for no effect")]
        public FsmFloat xMin;

        [HasFloatSlider(0f, 1f)]
        [Tooltip("Setting only the x value. Overrides anchorMin x value if set. Set to none for no effect")]
        public FsmFloat yMin;

        private RectTransform _rt;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.anchorMax = null;
            this.anchorMin = null;
            this.xMax = null;
            this.yMax = null;
            this.xMin = null;
            this.yMin = null;
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
            if (!this.anchorMax.IsNone)
            {
                this.anchorMax.Value = this._rt.anchorMax;
            }
            if (!this.anchorMin.IsNone)
            {
                this.anchorMin.Value = this._rt.anchorMax;
            }
            if (!this.xMax.IsNone)
            {
                this.xMax.Value = this._rt.anchorMax.x;
            }
            if (!this.yMax.IsNone)
            {
                this.yMax.Value = this._rt.anchorMax.y;
            }
            if (!this.xMin.IsNone)
            {
                this.xMin.Value = this._rt.anchorMin.x;
            }
            if (!this.yMin.IsNone)
            {
                this.yMin.Value = this._rt.anchorMin.y;
            }
        }
    }
}
