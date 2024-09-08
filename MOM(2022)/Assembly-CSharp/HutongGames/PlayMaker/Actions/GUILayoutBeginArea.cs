namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("Begin a GUILayout block of GUI controls in a fixed screen area. NOTE: Block must end with a corresponding GUILayoutEndArea.")]
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

        public override unsafe void OnGUI()
        {
            Rect rect1;
            if (!this.screenRect.IsNone)
            {
                rect1 = this.screenRect.get_Value();
            }
            else
            {
                rect1 = new Rect();
            }
            this.rect = rect1;
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
                Rect* rectPtr1 = &this.rect;
                rectPtr1.x *= Screen.width;
                Rect* rectPtr2 = &this.rect;
                rectPtr2.width *= Screen.width;
                Rect* rectPtr3 = &this.rect;
                rectPtr3.y *= Screen.height;
                Rect* rectPtr4 = &this.rect;
                rectPtr4.height *= Screen.height;
            }
            GUILayout.BeginArea(this.rect, GUIContent.none, this.style.Value);
        }

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
    }
}

