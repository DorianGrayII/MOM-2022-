namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Quaternion), Tooltip("Get the quaternion from a quaternion multiplied by a quaternion.")]
    public class GetQuaternionMultipliedByQuaternion : QuaternionBaseAction
    {
        [RequiredField, Tooltip("The first quaternion to multiply")]
        public FsmQuaternion quaternionA;
        [RequiredField, Tooltip("The second quaternion to multiply")]
        public FsmQuaternion quaternionB;
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The resulting quaternion")]
        public FsmQuaternion result;

        private void DoQuatMult()
        {
            this.result.set_Value(this.quaternionA.get_Value() * this.quaternionB.get_Value());
        }

        public override void OnEnter()
        {
            this.DoQuatMult();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
            {
                this.DoQuatMult();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
            {
                this.DoQuatMult();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
            {
                this.DoQuatMult();
            }
        }

        public override void Reset()
        {
            this.quaternionA = null;
            this.quaternionB = null;
            this.result = null;
            base.everyFrame = false;
            base.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
        }
    }
}

