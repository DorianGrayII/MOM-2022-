﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Camera), HutongGames.PlayMaker.Tooltip("Transforms position from world space into screen space. NOTE: Uses the MainCamera!")]
    public class WorldToScreenPoint : FsmStateAction
    {
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("World position to transform into screen coordinates.")]
        public FsmVector3 worldPosition;
        [HutongGames.PlayMaker.Tooltip("World X position.")]
        public FsmFloat worldX;
        [HutongGames.PlayMaker.Tooltip("World Y position.")]
        public FsmFloat worldY;
        [HutongGames.PlayMaker.Tooltip("World Z position.")]
        public FsmFloat worldZ;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the screen position in a Vector3 Variable. Z will equal zero.")]
        public FsmVector3 storeScreenPoint;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the screen X position in a Float Variable.")]
        public FsmFloat storeScreenX;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the screen Y position in a Float Variable.")]
        public FsmFloat storeScreenY;
        [HutongGames.PlayMaker.Tooltip("Normalize screen coordinates (0-1). Otherwise coordinates are in pixels.")]
        public FsmBool normalize;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
        public bool everyFrame;

        private unsafe void DoWorldToScreenPoint()
        {
            if (Camera.main == null)
            {
                base.LogError("No MainCamera defined!");
                base.Finish();
            }
            else
            {
                Vector3 zero = Vector3.zero;
                if (!this.worldPosition.IsNone)
                {
                    zero = this.worldPosition.get_Value();
                }
                if (!this.worldX.IsNone)
                {
                    zero.x = this.worldX.Value;
                }
                if (!this.worldY.IsNone)
                {
                    zero.y = this.worldY.Value;
                }
                if (!this.worldZ.IsNone)
                {
                    zero.z = this.worldZ.Value;
                }
                zero = Camera.main.WorldToScreenPoint(zero);
                if (this.normalize.Value)
                {
                    float* singlePtr1 = &zero.x;
                    singlePtr1[0] /= (float) Screen.width;
                    float* singlePtr2 = &zero.y;
                    singlePtr2[0] /= (float) Screen.height;
                }
                this.storeScreenPoint.set_Value(zero);
                this.storeScreenX.Value = zero.x;
                this.storeScreenY.Value = zero.y;
            }
        }

        public override void OnEnter()
        {
            this.DoWorldToScreenPoint();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoWorldToScreenPoint();
        }

        public override void Reset()
        {
            this.worldPosition = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.worldX = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.worldY = num2;
            FsmFloat num3 = new FsmFloat();
            num3.UseVariable = true;
            this.worldZ = num3;
            this.storeScreenPoint = null;
            this.storeScreenX = null;
            this.storeScreenY = null;
            this.everyFrame = false;
        }
    }
}

