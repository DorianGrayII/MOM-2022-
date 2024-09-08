namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Vector2), HutongGames.PlayMaker.Tooltip("Linearly interpolates between 2 vectors.")]
    public class Vector2Lerp : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("First Vector.")]
        public FsmVector2 fromVector;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Second Vector.")]
        public FsmVector2 toVector;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Interpolate between From Vector and ToVector by this amount. Value is clamped to 0-1 range. 0 = From Vector; 1 = To Vector; 0.5 = half way between.")]
        public FsmFloat amount;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the result in this vector variable.")]
        public FsmVector2 storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if any of the values are changing.")]
        public bool everyFrame;

        private void DoVector2Lerp()
        {
            this.storeResult.set_Value(Vector2.Lerp(this.fromVector.get_Value(), this.toVector.get_Value(), this.amount.Value));
        }

        public override void OnEnter()
        {
            this.DoVector2Lerp();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoVector2Lerp();
        }

        public override void Reset()
        {
            FsmVector2 vector1 = new FsmVector2();
            vector1.UseVariable = true;
            this.fromVector = vector1;
            FsmVector2 vector2 = new FsmVector2();
            vector2.UseVariable = true;
            this.toVector = vector2;
            this.storeResult = null;
            this.everyFrame = true;
        }
    }
}

