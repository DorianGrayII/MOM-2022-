using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Quaternion)]
    [Tooltip("Creates a rotation which rotates angle degrees around axis.")]
    public class QuaternionAngleAxis : QuaternionBaseAction
    {
        [RequiredField]
        [Tooltip("The angle.")]
        public FsmFloat angle;

        [RequiredField]
        [Tooltip("The rotation axis.")]
        public FsmVector3 axis;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the rotation of this quaternion variable.")]
        public FsmQuaternion result;

        public override void Reset()
        {
            this.angle = null;
            this.axis = null;
            this.result = null;
            base.everyFrame = true;
            base.everyFrameOption = everyFrameOptions.Update;
        }

        public override void OnEnter()
        {
            this.DoQuatAngleAxis();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.Update)
            {
                this.DoQuatAngleAxis();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.LateUpdate)
            {
                this.DoQuatAngleAxis();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.FixedUpdate)
            {
                this.DoQuatAngleAxis();
            }
        }

        private void DoQuatAngleAxis()
        {
            this.result.Value = Quaternion.AngleAxis(this.angle.Value, this.axis.Value);
        }
    }
}
