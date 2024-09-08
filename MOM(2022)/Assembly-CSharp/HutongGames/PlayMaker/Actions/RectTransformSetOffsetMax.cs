using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("\tThe offset of the upper right corner of the rectangle relative to the upper right anchor.")]
    public class RectTransformSetOffsetMax : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The Vector2 offsetMax. Set to none for no effect, and/or set individual axis below.")]
        public FsmVector2 offsetMax;

        [Tooltip("Setting only the x value. Overrides offsetMax x value if set. Set to none for no effect")]
        public FsmFloat x;

        [Tooltip("Setting only the y value. Overrides offsetMax y value if set. Set to none for no effect")]
        public FsmFloat y;

        private RectTransform _rt;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.offsetMax = null;
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
            this.DoSetOffsetMax();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.DoSetOffsetMax();
        }

        private void DoSetOffsetMax()
        {
            Vector2 value = this._rt.offsetMax;
            if (!this.offsetMax.IsNone)
            {
                value = this.offsetMax.Value;
            }
            if (!this.x.IsNone)
            {
                value.x = this.x.Value;
            }
            if (!this.y.IsNone)
            {
                value.y = this.y.Value;
            }
            this._rt.offsetMax = value;
        }
    }
}
