using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("The calculated rectangle in the local space of the Transform.")]
    public class RectTransformGetRect : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("The rect")]
        public FsmRect rect;

        [UIHint(UIHint.Variable)]
        public FsmFloat x;

        [UIHint(UIHint.Variable)]
        public FsmFloat y;

        [UIHint(UIHint.Variable)]
        public FsmFloat width;

        [UIHint(UIHint.Variable)]
        public FsmFloat height;

        private RectTransform _rt;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.rect = null;
            this.x = new FsmFloat
            {
                UseVariable = true
            };
            this.y = new FsmFloat
            {
                UseVariable = true
            };
            this.width = new FsmFloat
            {
                UseVariable = true
            };
            this.height = new FsmFloat
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
            if (!this.rect.IsNone)
            {
                this.rect.Value = this._rt.rect;
            }
            if (!this.x.IsNone)
            {
                this.x.Value = this._rt.rect.x;
            }
            if (!this.y.IsNone)
            {
                this.y.Value = this._rt.rect.y;
            }
            if (!this.width.IsNone)
            {
                this.width.Value = this._rt.rect.width;
            }
            if (!this.height.IsNone)
            {
                this.height.Value = this._rt.rect.height;
            }
        }
    }
}
