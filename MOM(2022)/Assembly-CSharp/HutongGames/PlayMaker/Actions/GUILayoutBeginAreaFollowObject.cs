namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("Begin a GUILayout area that follows the specified game object. Useful for overlays (e.g., playerName). NOTE: Block must end with a corresponding GUILayoutEndArea.")]
    public class GUILayoutBeginAreaFollowObject : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to follow.")]
        public FsmGameObject gameObject;
        [RequiredField]
        public FsmFloat offsetLeft;
        [RequiredField]
        public FsmFloat offsetTop;
        [RequiredField]
        public FsmFloat width;
        [RequiredField]
        public FsmFloat height;
        [HutongGames.PlayMaker.Tooltip("Use normalized screen coordinates (0-1).")]
        public FsmBool normalized;
        [HutongGames.PlayMaker.Tooltip("Optional named style in the current GUISkin")]
        public FsmString style;

        private static void DummyBeginArea()
        {
            Rect screenRect = new Rect();
            GUILayout.BeginArea(screenRect);
        }

        public override unsafe void OnGUI()
        {
            GameObject obj2 = this.gameObject.get_Value();
            if ((obj2 == null) || (Camera.main == null))
            {
                DummyBeginArea();
            }
            else
            {
                Vector3 position = obj2.transform.position;
                if (Camera.main.transform.InverseTransformPoint(position).z < 0f)
                {
                    DummyBeginArea();
                }
                else
                {
                    Vector2 vector1 = Camera.main.WorldToScreenPoint(position);
                    float x = vector1.x + (this.normalized.Value ? (this.offsetLeft.Value * Screen.width) : this.offsetLeft.Value);
                    Rect screenRect = new Rect(x, vector1.y + (this.normalized.Value ? (this.offsetTop.Value * Screen.width) : this.offsetTop.Value), this.width.Value, this.height.Value);
                    if (this.normalized.Value)
                    {
                        Rect* rectPtr1 = &screenRect;
                        rectPtr1.width *= Screen.width;
                        Rect* rectPtr2 = &screenRect;
                        rectPtr2.height *= Screen.height;
                    }
                    screenRect.y = Screen.height - screenRect.y;
                    GUILayout.BeginArea(screenRect, this.style.Value);
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.offsetLeft = 0f;
            this.offsetTop = 0f;
            this.width = 1f;
            this.height = 1f;
            this.normalized = true;
            this.style = "";
        }
    }
}

