using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Trigonometry)]
    [Tooltip("Get the cosine. You can use degrees, simply check on the DegToRad conversion")]
    public class GetCosine : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The angle. Note: You can use degrees, simply check DegtoRad if the angle is expressed in degrees.")]
        public FsmFloat angle;

        [Tooltip("Check on if the angle is expressed in degrees.")]
        public FsmBool DegToRad;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The angle cosine")]
        public FsmFloat result;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.angle = null;
            this.DegToRad = true;
            this.everyFrame = false;
            this.result = null;
        }

        public override void OnEnter()
        {
            this.DoCosine();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoCosine();
        }

        private void DoCosine()
        {
            float num = this.angle.Value;
            if (this.DegToRad.Value)
            {
                num *= (float)Math.PI / 180f;
            }
            this.result.Value = Mathf.Cos(num);
        }
    }
}
