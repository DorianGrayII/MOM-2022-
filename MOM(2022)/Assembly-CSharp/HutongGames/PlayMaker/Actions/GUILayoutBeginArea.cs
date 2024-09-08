using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GUILayout)]
    [Tooltip("Begin a GUILayout block of GUI controls in a fixed screen area. NOTE: Block must end with a corresponding GUILayoutEndArea.")]
    public class GUILayoutBeginArea : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        public FsmRect screenRect;

        public FsmFloat left;

        public FsmFloat top;

        public FsmFloat width;

        public FsmFloat height;

        public FsmBool normalized;

        public FsmString style;

        private Rect rect;

        public override void Reset()
        {
            this.screenRect = null;
            this.left = 0f;
            this.top = 0f;
            this.width = 1f;
            this.height = 1f;
            this.normalized = true;
            this.style = "";
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
            GUILayout.BeginArea(this.rect, GUIContent.none, this.style.Value);
        }
    }
}
