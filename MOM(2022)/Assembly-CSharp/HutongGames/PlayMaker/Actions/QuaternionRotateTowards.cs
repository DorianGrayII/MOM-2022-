namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Quaternion), HutongGames.PlayMaker.Tooltip("Rotates a rotation from towards to. This is essentially the same as Quaternion.Slerp but instead the function will ensure that the angular speed never exceeds maxDegreesDelta. Negative values of maxDegreesDelta pushes the rotation away from to.")]
    public class QuaternionRotateTowards : QuaternionBaseAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("From Quaternion.")]
        public FsmQuaternion fromQuaternion;
        [RequiredField, HutongGames.PlayMaker.Tooltip("To Quaternion.")]
        public FsmQuaternion toQuaternion;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The angular speed never exceeds maxDegreesDelta. Negative values of maxDegreesDelta pushes the rotation away from to.")]
        public FsmFloat maxDegreesDelta;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the result in this quaternion variable.")]
        public FsmQuaternion storeResult;

        private void DoQuatRotateTowards()
        {
            this.storeResult.set_Value(Quaternion.RotateTowards(this.fromQuaternion.get_Value(), this.toQuaternion.get_Value(), this.maxDegreesDelta.Value));
        }

        public override void OnEnter()
        {
            this.DoQuatRotateTowards();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
            {
                this.DoQuatRotateTowards();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
            {
                this.DoQuatRotateTowards();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
            {
                this.DoQuatRotateTowards();
            }
        }

        public override void Reset()
        {
            FsmQuaternion quaternion1 = new FsmQuaternion();
            quaternion1.UseVariable = true;
            this.fromQuaternion = quaternion1;
            FsmQuaternion quaternion2 = new FsmQuaternion();
            quaternion2.UseVariable = true;
            this.toQuaternion = quaternion2;
            this.maxDegreesDelta = 10f;
            this.storeResult = null;
            base.everyFrame = true;
            base.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
        }
    }
}

