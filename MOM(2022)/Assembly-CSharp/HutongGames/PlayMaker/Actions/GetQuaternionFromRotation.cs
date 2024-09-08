using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Quaternion)]
    [Tooltip("Creates a rotation which rotates from fromDirection to toDirection. Usually you use this to rotate a transform so that one of its axes, e.g., the y-axis - follows a target direction toDirection in world space.")]
    public class GetQuaternionFromRotation : QuaternionBaseAction
    {
        [RequiredField]
        [Tooltip("the 'from' direction")]
        public FsmVector3 fromDirection;

        [RequiredField]
        [Tooltip("the 'to' direction")]
        public FsmVector3 toDirection;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("the resulting quaternion")]
        public FsmQuaternion result;

        public override void Reset()
        {
            this.fromDirection = null;
            this.toDirection = null;
            this.result = null;
            base.everyFrame = false;
            base.everyFrameOption = everyFrameOptions.Update;
        }

        public override void OnEnter()
        {
            this.DoQuatFromRotation();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.Update)
            {
                this.DoQuatFromRotation();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.LateUpdate)
            {
                this.DoQuatFromRotation();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.FixedUpdate)
            {
                this.DoQuatFromRotation();
            }
        }

        private void DoQuatFromRotation()
        {
            this.result.Value = Quaternion.FromToRotation(this.fromDirection.Value, this.toDirection.Value);
        }
    }
}
