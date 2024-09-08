namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Quaternion), HutongGames.PlayMaker.Tooltip("Get the vector3 from a quaternion multiplied by a vector.")]
    public class GetQuaternionMultipliedByVector : QuaternionBaseAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The quaternion to multiply")]
        public FsmQuaternion quaternion;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The vector3 to multiply")]
        public FsmVector3 vector3;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The resulting vector3")]
        public FsmVector3 result;

        private void DoQuatMult()
        {
            this.result.set_Value((Vector3) (this.quaternion.get_Value() * this.vector3.get_Value()));
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
            this.quaternion = null;
            this.vector3 = null;
            this.result = null;
            base.everyFrame = false;
            base.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
        }
    }
}

