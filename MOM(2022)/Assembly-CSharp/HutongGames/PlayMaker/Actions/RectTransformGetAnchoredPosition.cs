using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("Get the position of the pivot of this RectTransform relative to the anchor reference point.")]
    public class RectTransformGetAnchoredPosition : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The anchored Position")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 position;

        [Tooltip("The x component of the anchored Position")]
        [UIHint(UIHint.Variable)]
        public FsmFloat x;

        [Tooltip("The y component of the anchored Position")]
        [UIHint(UIHint.Variable)]
        public FsmFloat y;

        private RectTransform _rt;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.position = null;
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
            if (!this.position.IsNone)
            {
                this.position.Value = this._rt.anchoredPosition;
            }
            if (!this.x.IsNone)
            {
                this.x.Value = this._rt.anchoredPosition.x;
            }
            if (!this.y.IsNone)
            {
                this.y.Value = this._rt.anchoredPosition.y;
            }
        }
    }
}
