namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Quaternion), HutongGames.PlayMaker.Tooltip("Creates a rotation that looks along forward with the head upwards along upwards.")]
    public class QuaternionLookRotation : QuaternionBaseAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("the rotation direction")]
        public FsmVector3 direction;
        [HutongGames.PlayMaker.Tooltip("The up direction")]
        public FsmVector3 upVector;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the inverse of the rotation variable.")]
        public FsmQuaternion result;

        private void DoQuatLookRotation()
        {
            if (!this.upVector.IsNone)
            {
                this.result.set_Value(Quaternion.LookRotation(this.direction.get_Value(), this.upVector.get_Value()));
            }
            else
            {
                this.result.set_Value(Quaternion.LookRotation(this.direction.get_Value()));
            }
        }

        public override void OnEnter()
        {
            this.DoQuatLookRotation();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
            {
                this.DoQuatLookRotation();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
            {
                this.DoQuatLookRotation();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
            {
                this.DoQuatLookRotation();
            }
        }

        public override void Reset()
        {
            this.direction = null;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.upVector = vector1;
            this.result = null;
            base.everyFrame = true;
            base.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
        }
    }
}

