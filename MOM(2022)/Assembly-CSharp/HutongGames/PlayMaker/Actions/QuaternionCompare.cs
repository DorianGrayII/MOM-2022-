using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Quaternion)]
    [Tooltip("Check if two quaternions are equals or not. Takes in account inversed representations of quaternions")]
    public class QuaternionCompare : QuaternionBaseAction
    {
        [RequiredField]
        [Tooltip("First Quaternion")]
        public FsmQuaternion Quaternion1;

        [RequiredField]
        [Tooltip("Second Quaternion")]
        public FsmQuaternion Quaternion2;

        [Tooltip("true if Quaternions are equal")]
        public FsmBool equal;

        [Tooltip("Event sent if Quaternions are equal")]
        public FsmEvent equalEvent;

        [Tooltip("Event sent if Quaternions are not equal")]
        public FsmEvent notEqualEvent;

        public override void Reset()
        {
            this.Quaternion1 = new FsmQuaternion
            {
                UseVariable = true
            };
            this.Quaternion2 = new FsmQuaternion
            {
                UseVariable = true
            };
            this.equal = null;
            this.equalEvent = null;
            this.notEqualEvent = null;
            base.everyFrameOption = everyFrameOptions.Update;
        }

        public override void OnEnter()
        {
            this.DoQuatCompare();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.Update)
            {
                this.DoQuatCompare();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.LateUpdate)
            {
                this.DoQuatCompare();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.FixedUpdate)
            {
                this.DoQuatCompare();
            }
        }

        private void DoQuatCompare()
        {
            bool flag = Mathf.Abs(Quaternion.Dot(this.Quaternion1.Value, this.Quaternion2.Value)) > 0.999999f;
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
    }
}
