using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Vector2)]
    [Tooltip("Linearly interpolates between 2 vectors.")]
    public class Vector2Lerp : FsmStateAction
    {
        [RequiredField]
        [Tooltip("First Vector.")]
        public FsmVector2 fromVector;

        [RequiredField]
        [Tooltip("Second Vector.")]
        public FsmVector2 toVector;

        [RequiredField]
        [Tooltip("Interpolate between From Vector and ToVector by this amount. Value is clamped to 0-1 range. 0 = From Vector; 1 = To Vector; 0.5 = half way between.")]
        public FsmFloat amount;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in this vector variable.")]
        public FsmVector2 storeResult;

        [Tooltip("Repeat every frame. Useful if any of the values are changing.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.fromVector = new FsmVector2
            {
                UseVariable = true
            };
            this.toVector = new FsmVector2
            {
                UseVariable = true
            };
            this.storeResult = null;
            this.everyFrame = true;
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

        private void DoVector2Lerp()
        {
            this.storeResult.Value = Vector2.Lerp(this.fromVector.Value, this.toVector.Value, this.amount.Value);
        }
    }
}
