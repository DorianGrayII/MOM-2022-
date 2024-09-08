using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Quaternion)]
    [Tooltip("Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis (in that order).")]
    public class QuaternionEuler : QuaternionBaseAction
    {
        [RequiredField]
        [Tooltip("The Euler angles.")]
        public FsmVector3 eulerAngles;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the euler angles of this quaternion variable.")]
        public FsmQuaternion result;

        public override void Reset()
        {
            this.eulerAngles = null;
            this.result = null;
            base.everyFrame = true;
            base.everyFrameOption = everyFrameOptions.Update;
        }

        public override void OnEnter()
        {
            this.DoQuatEuler();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.Update)
            {
                this.DoQuatEuler();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.LateUpdate)
            {
                this.DoQuatEuler();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.FixedUpdate)
            {
                this.DoQuatEuler();
            }
        }

        private void DoQuatEuler()
        {
            this.result.Value = Quaternion.Euler(this.eulerAngles.Value);
        }
    }
}
