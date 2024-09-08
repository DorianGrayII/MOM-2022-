namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Vector3), HutongGames.PlayMaker.Tooltip("Linearly interpolates between 2 vectors.")]
    public class Vector3Lerp : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("First Vector.")]
        public FsmVector3 fromVector;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Second Vector.")]
        public FsmVector3 toVector;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Interpolate between From Vector and ToVector by this amount. Value is clamped to 0-1 range. 0 = From Vector; 1 = To Vector; 0.5 = half way between.")]
        public FsmFloat amount;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the result in this vector variable.")]
        public FsmVector3 storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if any of the values are changing.")]
        public bool everyFrame;

        private void DoVector3Lerp()
        {
            this.storeResult.set_Value(Vector3.Lerp(this.fromVector.get_Value(), this.toVector.get_Value(), this.amount.Value));
        }

        public override void OnEnter()
        {
            this.DoVector3Lerp();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoVector3Lerp();
        }

        public override void Reset()
        {
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.fromVector = vector1;
            FsmVector3 vector2 = new FsmVector3();
            vector2.UseVariable = true;
            this.toVector = vector2;
            this.storeResult = null;
            this.everyFrame = true;
        }
    }
}

