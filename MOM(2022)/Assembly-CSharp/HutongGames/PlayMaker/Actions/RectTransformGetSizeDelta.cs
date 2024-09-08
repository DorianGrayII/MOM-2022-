using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("Get the size of this RectTransform relative to the distances between the anchors. this is the 'Width' and 'Height' values in the RectTransform inspector.")]
    public class RectTransformGetSizeDelta : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The sizeDelta")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 sizeDelta;

        [Tooltip("The x component of the sizeDelta, the width.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat width;

        [Tooltip("The y component of the sizeDelta, the height")]
        [UIHint(UIHint.Variable)]
        public FsmFloat height;

        private RectTransform _rt;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.sizeDelta = null;
            this.width = null;
            this.height = null;
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
            if (!this.sizeDelta.IsNone)
            {
                this.sizeDelta.Value = this._rt.sizeDelta;
            }
            if (!this.width.IsNone)
            {
                this.width.Value = this._rt.sizeDelta.x;
            }
            if (!this.height.IsNone)
            {
                this.height.Value = this._rt.sizeDelta.y;
            }
        }
    }
}
