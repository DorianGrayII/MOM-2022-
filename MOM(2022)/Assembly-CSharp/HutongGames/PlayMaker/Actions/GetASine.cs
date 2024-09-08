﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Trigonometry), HutongGames.PlayMaker.Tooltip("Get the Arc sine. You can get the result in degrees, simply check on the RadToDeg conversion")]
    public class GetASine : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The value of the sine")]
        public FsmFloat Value;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The resulting angle. Note:If you want degrees, simply check RadToDeg")]
        public FsmFloat angle;
        [HutongGames.PlayMaker.Tooltip("Check on if you want the angle expressed in degrees.")]
        public FsmBool RadToDeg;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoASine()
        {
            float num = Mathf.Asin(this.Value.Value);
            if (this.RadToDeg.Value)
            {
                num *= 57.29578f;
            }
            this.angle.Value = num;
        }

        public override void OnEnter()
        {
            this.DoASine();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoASine();
        }

        public override void Reset()
        {
            this.angle = null;
            this.RadToDeg = true;
            this.everyFrame = false;
            this.Value = null;
        }
    }
}

