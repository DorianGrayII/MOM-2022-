using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("Get the normalized position in the parent RectTransform that the upper right corner is anchored to.")]
    public class RectTransformGetAnchorMax : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The anchorMax")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 anchorMax;

        [Tooltip("The x component of the anchorMax")]
        [UIHint(UIHint.Variable)]
        public FsmFloat x;

        [Tooltip("The y component of the anchorMax")]
        [UIHint(UIHint.Variable)]
        public FsmFloat y;

        private RectTransform _rt;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.anchorMax = null;
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
            if (!this.anchorMax.IsNone)
            {
                this.anchorMax.Value = this._rt.anchorMax;
            }
            if (!this.x.IsNone)
            {
                this.x.Value = this._rt.anchorMax.x;
            }
            if (!this.y.IsNone)
            {
                this.y.Value = this._rt.anchorMax.y;
            }
        }
    }
}
