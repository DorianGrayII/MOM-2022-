using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("Get the normalized position in the parent RectTransform that the lower left corner is anchored to.")]
    public class RectTransformGetAnchorMin : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The anchorMin")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 anchorMin;

        [Tooltip("The x component of the anchorMin")]
        [UIHint(UIHint.Variable)]
        public FsmFloat x;

        [Tooltip("The y component of the anchorMin")]
        [UIHint(UIHint.Variable)]
        public FsmFloat y;

        private RectTransform _rt;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.anchorMin = null;
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
            if (!this.anchorMin.IsNone)
            {
                this.anchorMin.Value = this._rt.anchorMin;
            }
            if (!this.x.IsNone)
            {
                this.x.Value = this._rt.anchorMin.x;
            }
            if (!this.y.IsNone)
            {
                this.y.Value = this._rt.anchorMin.y;
            }
        }
    }
}
