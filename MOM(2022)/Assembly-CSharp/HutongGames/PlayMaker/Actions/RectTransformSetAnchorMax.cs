using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("The normalized position in the parent RectTransform that the upper right corner is anchored to. This is relative screen space, values ranges from 0 to 1")]
    public class RectTransformSetAnchorMax : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The Vector2 anchor. Set to none for no effect, and/or set individual axis below.")]
        public FsmVector2 anchorMax;

        [HasFloatSlider(0f, 1f)]
        [Tooltip("Setting only the x value. Overrides anchorMax x value if set. Set to none for no effect")]
        public FsmFloat x;

        [HasFloatSlider(0f, 1f)]
        [Tooltip("Setting only the x value. Overrides anchorMax x value if set. Set to none for no effect")]
        public FsmFloat y;

        private RectTransform _rt;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.anchorMax = null;
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
            if (!this.anchorMax.IsNone)
            {
                value = this.anchorMax.Value;
            }
            if (!this.x.IsNone)
            {
                value.x = this.x.Value;
            }
            if (!this.y.IsNone)
            {
                value.y = this.y.Value;
            }
            this._rt.anchorMax = value;
        }
    }
}
