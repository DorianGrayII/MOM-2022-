using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("Get the offset of the lower left corner of the rectangle relative to the lower left anchor")]
    public class RectTransformGetOffsetMin : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The offsetMin")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 offsetMin;

        [Tooltip("The x component of the offsetMin")]
        [UIHint(UIHint.Variable)]
        public FsmFloat x;

        [Tooltip("The y component of the offsetMin")]
        [UIHint(UIHint.Variable)]
        public FsmFloat y;

        private RectTransform _rt;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.offsetMin = null;
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
            if (!this.offsetMin.IsNone)
            {
                this.offsetMin.Value = this._rt.offsetMin;
            }
            if (!this.x.IsNone)
            {
                this.x.Value = this._rt.offsetMin.x;
            }
            if (!this.y.IsNone)
            {
                this.y.Value = this._rt.offsetMin.y;
            }
        }
    }
}
