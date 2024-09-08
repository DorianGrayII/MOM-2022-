using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("GUI base action - don't use!")]
    public abstract class GUIAction : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        public FsmRect screenRect;

        public FsmFloat left;

        public FsmFloat top;

        public FsmFloat width;

        public FsmFloat height;

        [RequiredField]
        public FsmBool normalized;

        internal Rect rect;

        public override void Reset()
        {
            this.screenRect = null;
            this.left = 0f;
            this.top = 0f;
            this.width = 1f;
            this.height = 1f;
            this.normalized = true;
        }

        public override void OnGUI()
        {
            this.rect = ((!this.screenRect.IsNone) ? this.screenRect.Value : default(Rect));
            if (!this.left.IsNone)
            {
                this.rect.x = this.left.Value;
            }
            if (!this.top.IsNone)
            {
                this.rect.y = this.top.Value;
            }
            if (!this.width.IsNone)
            {
                this.rect.width = this.width.Value;
            }
            if (!this.height.IsNone)
            {
                this.rect.height = this.height.Value;
            }
            if (this.normalized.Value)
            {
                this.rect.x *= Screen.width;
                this.rect.width *= Screen.width;
                this.rect.y *= Screen.height;
                this.rect.height *= Screen.height;
            }
        }
    }
}
