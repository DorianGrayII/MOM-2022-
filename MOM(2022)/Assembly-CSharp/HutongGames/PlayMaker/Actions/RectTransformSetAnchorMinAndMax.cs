using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("The normalized position in the parent RectTransform that the upper right corner is anchored to. This is relative screen space, values ranges from 0 to 1")]
    public class RectTransformSetAnchorMinAndMax : BaseUpdateAction
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
            this.xMax = new FsmFloat
            {
                UseVariable = true
            };
            this.yMax = new FsmFloat
            {
                UseVariable = true
            };
            this.xMin = new FsmFloat
            {
                UseVariable = true
            };
            this.yMin = new FsmFloat
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
            this.DoSetAnchorMax();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.DoSetAnchorMax();
        }

        private void DoSetAnchorMax()
        {
            Vector2 value = this._rt.anchorMax;
            Vector2 value2 = this._rt.anchorMin;
            if (!this.anchorMax.IsNone)
            {
                value = this.anchorMax.Value;
                value2 = this.anchorMin.Value;
            }
            if (!this.xMax.IsNone)
            {
                value.x = this.xMax.Value;
            }
            if (!this.yMax.IsNone)
            {
                value.y = this.yMax.Value;
            }
            if (!this.xMin.IsNone)
            {
                value2.x = this.xMin.Value;
            }
            if (!this.yMin.IsNone)
            {
                value2.y = this.yMin.Value;
            }
            this._rt.anchorMax = value;
            this._rt.anchorMin = value2;
        }
    }
}
