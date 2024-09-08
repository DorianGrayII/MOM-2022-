namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Quaternion), HutongGames.PlayMaker.Tooltip("Check if two quaternions are equals or not. Takes in account inversed representations of quaternions")]
    public class QuaternionCompare : QuaternionBaseAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("First Quaternion")]
        public FsmQuaternion Quaternion1;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Second Quaternion")]
        public FsmQuaternion Quaternion2;
        [HutongGames.PlayMaker.Tooltip("true if Quaternions are equal")]
        public FsmBool equal;
        [HutongGames.PlayMaker.Tooltip("Event sent if Quaternions are equal")]
        public FsmEvent equalEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if Quaternions are not equal")]
        public FsmEvent notEqualEvent;

        private void DoQuatCompare()
        {
            bool flag = Mathf.Abs(Quaternion.Dot(this.Quaternion1.get_Value(), this.Quaternion2.get_Value())) > 0.999999f;
            this.equal.Value = flag;
            if (flag)
            {
                base.Fsm.Event(this.equalEvent);
            }
            else
            {
                base.Fsm.Event(this.notEqualEvent);
            }
        }

        public override void OnEnter()
        {
            this.DoQuatCompare();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
            {
                this.DoQuatCompare();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
            {
                this.DoQuatCompare();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
            {
                this.DoQuatCompare();
            }
        }

        public override void Reset()
        {
            FsmQuaternion quaternion1 = new FsmQuaternion();
            quaternion1.UseVariable = true;
            this.Quaternion1 = quaternion1;
            FsmQuaternion quaternion2 = new FsmQuaternion();
            quaternion2.UseVariable = true;
            this.Quaternion2 = quaternion2;
            this.equal = null;
            this.equalEvent = null;
            this.notEqualEvent = null;
            base.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
        }
    }
}

