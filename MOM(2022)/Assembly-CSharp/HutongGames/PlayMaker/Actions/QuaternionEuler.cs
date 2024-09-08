namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Quaternion), HutongGames.PlayMaker.Tooltip("Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis (in that order).")]
    public class QuaternionEuler : QuaternionBaseAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The Euler angles.")]
        public FsmVector3 eulerAngles;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the euler angles of this quaternion variable.")]
        public FsmQuaternion result;

        private void DoQuatEuler()
        {
            this.result.set_Value(Quaternion.Euler(this.eulerAngles.get_Value()));
        }

        public override void OnEnter()
        {
            this.DoQuatEuler();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
            {
                this.DoQuatEuler();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
            {
                this.DoQuatEuler();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
            {
                this.DoQuatEuler();
            }
        }

        public override void Reset()
        {
            this.eulerAngles = null;
            this.result = null;
            base.everyFrame = true;
            base.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
        }
    }
}

