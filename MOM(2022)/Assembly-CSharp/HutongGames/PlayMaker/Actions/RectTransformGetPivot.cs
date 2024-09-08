using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("Get the normalized position in this RectTransform that it rotates around.")]
    public class RectTransformGetPivot : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The pivot")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 pivot;

        [Tooltip("The x component of the pivot")]
        [UIHint(UIHint.Variable)]
        public FsmFloat x;

        [Tooltip("The y component of the pivot")]
        [UIHint(UIHint.Variable)]
        public FsmFloat y;

        private RectTransform _rt;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.pivot = null;
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
            if (!this.pivot.IsNone)
            {
                this.pivot.Value = this._rt.pivot;
            }
            if (!this.x.IsNone)
            {
                this.x.Value = this._rt.pivot.x;
            }
            if (!this.y.IsNone)
            {
                this.y.Value = this._rt.pivot.y;
            }
        }
    }
}
