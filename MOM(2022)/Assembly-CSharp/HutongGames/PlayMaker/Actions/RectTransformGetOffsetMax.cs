using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("Get the offset of the upper right corner of the rectangle relative to the upper right anchor")]
    public class RectTransformGetOffsetMax : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The offsetMax")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 offsetMax;

        [Tooltip("The x component of the offsetMax")]
        [UIHint(UIHint.Variable)]
        public FsmFloat x;

        [Tooltip("The y component of the offsetMax")]
        [UIHint(UIHint.Variable)]
        public FsmFloat y;

        private RectTransform _rt;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.offsetMax = null;
            this.x = null;
            this.y = null;
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
            if (!this.offsetMax.IsNone)
            {
                this.offsetMax.Value = this._rt.offsetMax;
            }
            if (!this.x.IsNone)
            {
                this.x.Value = this._rt.offsetMax.x;
            }
            if (!this.y.IsNone)
            {
                this.y.Value = this._rt.offsetMax.y;
            }
        }
    }
}
