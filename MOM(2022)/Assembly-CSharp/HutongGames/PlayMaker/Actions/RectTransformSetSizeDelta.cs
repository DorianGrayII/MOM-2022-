using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("Set the size of this RectTransform relative to the distances between the anchors. this is the 'Width' and 'Height' values in the RectTransform inspector.")]
    public class RectTransformSetSizeDelta : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("TheVector2 sizeDelta. Set to none for no effect, and/or set individual axis below.")]
        public FsmVector2 sizeDelta;

        [Tooltip("Setting only the x value. Overrides sizeDelta x value if set. Set to none for no effect")]
        public FsmFloat x;

        [Tooltip("Setting only the x value. Overrides sizeDelta y value if set. Set to none for no effect")]
        public FsmFloat y;

        private RectTransform _rt;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.sizeDelta = null;
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
            this.DoSetSizeDelta();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.DoSetSizeDelta();
        }

        private void DoSetSizeDelta()
        {
            Vector2 value = this._rt.sizeDelta;
            if (!this.sizeDelta.IsNone)
            {
                value = this.sizeDelta.Value;
            }
            if (!this.x.IsNone)
            {
                value.x = this.x.Value;
            }
            if (!this.y.IsNone)
            {
                value.y = this.y.Value;
            }
            this._rt.sizeDelta = value;
        }
    }
}
