using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Quaternion)]
    [Tooltip("Inverse a quaternion")]
    public class QuaternionInverse : QuaternionBaseAction
    {
        [RequiredField]
        [Tooltip("the rotation")]
        public FsmQuaternion rotation;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the inverse of the rotation variable.")]
        public FsmQuaternion result;

        public override void Reset()
        {
            this.rotation = null;
            this.result = null;
            base.everyFrame = true;
            base.everyFrameOption = everyFrameOptions.Update;
        }

        public override void OnEnter()
        {
            this.DoQuatInverse();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.Update)
            {
                this.DoQuatInverse();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.LateUpdate)
            {
                this.DoQuatInverse();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.FixedUpdate)
            {
                this.DoQuatInverse();
            }
        }

        private void DoQuatInverse()
        {
            this.result.Value = Quaternion.Inverse(this.rotation.Value);
        }
    }
}
