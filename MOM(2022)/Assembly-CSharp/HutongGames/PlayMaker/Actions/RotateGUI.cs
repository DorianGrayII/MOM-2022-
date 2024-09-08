namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("Rotates the GUI around a pivot point. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
    public class RotateGUI : FsmStateAction
    {
        [RequiredField]
        public FsmFloat angle;
        [RequiredField]
        public FsmFloat pivotX;
        [RequiredField]
        public FsmFloat pivotY;
        public bool normalized;
        public bool applyGlobally;
        private bool applied;

        public override unsafe void OnGUI()
        {
            if (!this.applied)
            {
                Vector2 pivotPoint = new Vector2(this.pivotX.Value, this.pivotY.Value);
                if (this.normalized)
                {
                    float* singlePtr1 = &pivotPoint.x;
                    singlePtr1[0] *= Screen.width;
                    float* singlePtr2 = &pivotPoint.y;
                    singlePtr2[0] *= Screen.height;
                }
                GUIUtility.RotateAroundPivot(this.angle.Value, pivotPoint);
                if (this.applyGlobally)
                {
                    PlayMakerGUI.set_GUIMatrix(GUI.matrix);
                    this.applied = true;
                }
            }
        }

        public override void OnUpdate()
        {
            this.applied = false;
        }

        public override void Reset()
        {
            this.angle = 0f;
            this.pivotX = 0.5f;
            this.pivotY = 0.5f;
            this.normalized = true;
            this.applyGlobally = false;
        }
    }
}

