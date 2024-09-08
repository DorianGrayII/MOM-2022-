using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("The normalized position in this RectTransform that it rotates around.")]
    public class RectTransformSetPivot : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The Vector2 pivot. Set to none for no effect, and/or set individual axis below.")]
        public FsmVector2 pivot;

        [HasFloatSlider(0f, 1f)]
        [Tooltip("Setting only the x value. Overrides pivot x value if set. Set to none for no effect")]
        public FsmFloat x;

        [HasFloatSlider(0f, 1f)]
        [Tooltip("Setting only the x value. Overrides pivot y value if set. Set to none for no effect")]
        public FsmFloat y;

        private RectTransform _rt;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.pivot = null;
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
            this.DoSetPivotPosition();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.DoSetPivotPosition();
        }

        private void DoSetPivotPosition()
        {
            Vector2 value = this._rt.pivot;
            if (!this.pivot.IsNone)
            {
                value = this.pivot.Value;
            }
            if (!this.x.IsNone)
            {
                value.x = this.x.Value;
            }
            if (!this.y.IsNone)
            {
                value.y = this.y.Value;
            }
            this._rt.pivot = value;
        }
    }
}
