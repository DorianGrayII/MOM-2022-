namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Trigonometry), HutongGames.PlayMaker.Tooltip("Get the sine. You can use degrees, simply check on the DegToRad conversion")]
    public class GetSine : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The angle. Note: You can use degrees, simply check DegtoRad if the angle is expressed in degrees.")]
        public FsmFloat angle;
        [HutongGames.PlayMaker.Tooltip("Check on if the angle is expressed in degrees.")]
        public FsmBool DegToRad;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The angle tan")]
        public FsmFloat result;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoSine()
        {
            float f = this.angle.Value;
            if (this.DegToRad.Value)
            {
                f *= 0.01745329f;
            }
            this.result.Value = Mathf.Sin(f);
        }

        public override void OnEnter()
        {
            this.DoSine();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSine();
        }

        public override void Reset()
        {
            this.angle = null;
            this.DegToRad = true;
            this.everyFrame = false;
            this.result = null;
        }
    }
}

