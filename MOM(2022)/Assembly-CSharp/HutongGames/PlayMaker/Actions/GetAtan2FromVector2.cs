﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Trigonometry), HutongGames.PlayMaker.Tooltip("Get the Arc Tangent 2 as in atan2(y,x) from a vector 2. You can get the result in degrees, simply check on the RadToDeg conversion")]
    public class GetAtan2FromVector2 : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The vector2 of the tan")]
        public FsmVector2 vector2;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The resulting angle. Note:If you want degrees, simply check RadToDeg")]
        public FsmFloat angle;
        [HutongGames.PlayMaker.Tooltip("Check on if you want the angle expressed in degrees.")]
        public FsmBool RadToDeg;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoATan()
        {
            float num = Mathf.Atan2(this.vector2.get_Value().y, this.vector2.get_Value().x);
            if (this.RadToDeg.Value)
            {
                num *= 57.29578f;
            }
            this.angle.Value = num;
        }

        public override void OnEnter()
        {
            this.DoATan();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoATan();
        }

        public override void Reset()
        {
            this.vector2 = null;
            this.RadToDeg = true;
            this.everyFrame = false;
            this.angle = null;
        }
    }
}

