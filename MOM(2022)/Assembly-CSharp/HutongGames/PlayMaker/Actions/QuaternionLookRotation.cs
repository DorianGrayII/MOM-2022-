using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Quaternion)]
    [Tooltip("Creates a rotation that looks along forward with the head upwards along upwards.")]
    public class QuaternionLookRotation : QuaternionBaseAction
    {
        [RequiredField]
        [Tooltip("the rotation direction")]
        public FsmVector3 direction;

        [Tooltip("The up direction")]
        public FsmVector3 upVector;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the inverse of the rotation variable.")]
        public FsmQuaternion result;

        public override void Reset()
        {
            this.direction = null;
            this.upVector = new FsmVector3
            {
                UseVariable = true
            };
            this.result = null;
            base.everyFrame = true;
            base.everyFrameOption = everyFrameOptions.Update;
        }

        public override void OnEnter()
        {
            this.DoQuatLookRotation();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.Update)
            {
                this.DoQuatLookRotation();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.LateUpdate)
            {
                this.DoQuatLookRotation();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.FixedUpdate)
            {
                this.DoQuatLookRotation();
            }
        }

        private void DoQuatLookRotation()
        {
            if (!this.upVector.IsNone)
            {
                this.result.Value = Quaternion.LookRotation(this.direction.Value, this.upVector.Value);
            }
            else
            {
                this.result.Value = Quaternion.LookRotation(this.direction.Value);
            }
        }
    }
}
