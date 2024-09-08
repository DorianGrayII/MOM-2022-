namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Quaternion), HutongGames.PlayMaker.Tooltip("Creates a rotation which rotates angle degrees around axis.")]
    public class QuaternionAngleAxis : QuaternionBaseAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The angle.")]
        public FsmFloat angle;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The rotation axis.")]
        public FsmVector3 axis;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the rotation of this quaternion variable.")]
        public FsmQuaternion result;

        private void DoQuatAngleAxis()
        {
            this.result.set_Value(Quaternion.AngleAxis(this.angle.Value, this.axis.get_Value()));
        }

        public override void OnEnter()
        {
            this.DoQuatAngleAxis();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
            {
                this.DoQuatAngleAxis();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
            {
                this.DoQuatAngleAxis();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
            {
                this.DoQuatAngleAxis();
            }
        }

        public override void Reset()
        {
            this.angle = null;
            this.axis = null;
            this.result = null;
            base.everyFrame = true;
            base.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
        }
    }
}

