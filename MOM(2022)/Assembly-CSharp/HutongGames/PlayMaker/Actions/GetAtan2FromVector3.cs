namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Trigonometry), HutongGames.PlayMaker.Tooltip("Get the Arc Tangent 2 as in atan2(y,x) from a vector 3, where you pick which is x and y from the vector 3. You can get the result in degrees, simply check on the RadToDeg conversion")]
    public class GetAtan2FromVector3 : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The vector3 definition of the tan")]
        public FsmVector3 vector3;
        [RequiredField, HutongGames.PlayMaker.Tooltip("which axis in the vector3 to use as the x value of the tan")]
        public aTan2EnumAxis xAxis;
        [RequiredField, HutongGames.PlayMaker.Tooltip("which axis in the vector3 to use as the y value of the tan")]
        public aTan2EnumAxis yAxis;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The resulting angle. Note:If you want degrees, simply check RadToDeg")]
        public FsmFloat angle;
        [HutongGames.PlayMaker.Tooltip("Check on if you want the angle expressed in degrees.")]
        public FsmBool RadToDeg;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoATan()
        {
            float x = this.vector3.get_Value().x;
            if (this.xAxis == aTan2EnumAxis.y)
            {
                x = this.vector3.get_Value().y;
            }
            else if (this.xAxis == aTan2EnumAxis.z)
            {
                x = this.vector3.get_Value().z;
            }
            float y = this.vector3.get_Value().y;
            if (this.yAxis == aTan2EnumAxis.x)
            {
                y = this.vector3.get_Value().x;
            }
            else if (this.yAxis == aTan2EnumAxis.z)
            {
                y = this.vector3.get_Value().z;
            }
            float num3 = Mathf.Atan2(y, x);
            if (this.RadToDeg.Value)
            {
                num3 *= 57.29578f;
            }
            this.angle.Value = num3;
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
            this.vector3 = null;
            this.xAxis = aTan2EnumAxis.x;
            this.yAxis = aTan2EnumAxis.y;
            this.RadToDeg = true;
            this.everyFrame = false;
            this.angle = null;
        }

        public enum aTan2EnumAxis
        {
            x,
            y,
            z
        }
    }
}

