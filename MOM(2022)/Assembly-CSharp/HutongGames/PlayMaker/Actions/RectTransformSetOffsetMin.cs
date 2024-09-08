using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("The offset of the lower left corner of the rectangle relative to the lower left anchor.")]
    public class RectTransformSetOffsetMin : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The Vector2 offsetMin. Set to none for no effect, and/or set individual axis below.")]
        public FsmVector2 offsetMin;

        [Tooltip("Setting only the x value. Overrides offsetMin x value if set. Set to none for no effect")]
        public FsmFloat x;

        [Tooltip("Setting only the x value. Overrides offsetMin y value if set. Set to none for no effect")]
        public FsmFloat y;

        private RectTransform _rt;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.offsetMin = null;
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
            this.DoSetOffsetMin();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.DoSetOffsetMin();
        }

        private void DoSetOffsetMin()
        {
            Vector2 value = this._rt.offsetMin;
            if (!this.offsetMin.IsNone)
            {
                value = this.offsetMin.Value;
            }
            if (!this.x.IsNone)
            {
                value.x = this.x.Value;
            }
            if (!this.y.IsNone)
            {
                value.y = this.y.Value;
            }
            this._rt.offsetMin = value;
        }
    }
}
