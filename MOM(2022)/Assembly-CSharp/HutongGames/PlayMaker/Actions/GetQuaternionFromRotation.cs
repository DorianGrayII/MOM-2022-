namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Quaternion), HutongGames.PlayMaker.Tooltip("Creates a rotation which rotates from fromDirection to toDirection. Usually you use this to rotate a transform so that one of its axes, e.g., the y-axis - follows a target direction toDirection in world space.")]
    public class GetQuaternionFromRotation : QuaternionBaseAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("the 'from' direction")]
        public FsmVector3 fromDirection;
        [RequiredField, HutongGames.PlayMaker.Tooltip("the 'to' direction")]
        public FsmVector3 toDirection;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("the resulting quaternion")]
        public FsmQuaternion result;

        private void DoQuatFromRotation()
        {
            this.result.set_Value(Quaternion.FromToRotation(this.fromDirection.get_Value(), this.toDirection.get_Value()));
        }

        public override void OnEnter()
        {
            this.DoQuatFromRotation();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
            {
                this.DoQuatFromRotation();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
            {
                this.DoQuatFromRotation();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
            {
                this.DoQuatFromRotation();
            }
        }

        public override void Reset()
        {
            this.fromDirection = null;
            this.toDirection = null;
            this.result = null;
            base.everyFrame = false;
            base.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
        }
    }
}

