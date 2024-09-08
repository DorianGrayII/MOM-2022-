using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Quaternion)]
    [Tooltip("Rotates a rotation from towards to. This is essentially the same as Quaternion.Slerp but instead the function will ensure that the angular speed never exceeds maxDegreesDelta. Negative values of maxDegreesDelta pushes the rotation away from to.")]
    public class QuaternionRotateTowards : QuaternionBaseAction
    {
        [RequiredField]
        [Tooltip("From Quaternion.")]
        public FsmQuaternion fromQuaternion;

        [RequiredField]
        [Tooltip("To Quaternion.")]
        public FsmQuaternion toQuaternion;

        [RequiredField]
        [Tooltip("The angular speed never exceeds maxDegreesDelta. Negative values of maxDegreesDelta pushes the rotation away from to.")]
        public FsmFloat maxDegreesDelta;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in this quaternion variable.")]
        public FsmQuaternion storeResult;

        public override void Reset()
        {
            this.fromQuaternion = new FsmQuaternion
            {
                UseVariable = true
            };
            this.toQuaternion = new FsmQuaternion
            {
                UseVariable = true
            };
            this.maxDegreesDelta = 10f;
            this.storeResult = null;
            base.everyFrame = true;
            base.everyFrameOption = everyFrameOptions.Update;
        }

        public override void OnEnter()
        {
            this.DoQuatRotateTowards();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.Update)
            {
                this.DoQuatRotateTowards();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.LateUpdate)
            {
                this.DoQuatRotateTowards();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.FixedUpdate)
            {
                this.DoQuatRotateTowards();
            }
        }

        private void DoQuatRotateTowards()
        {
            this.storeResult.Value = Quaternion.RotateTowards(this.fromQuaternion.Value, this.toQuaternion.Value, this.maxDegreesDelta.Value);
        }
    }
}
