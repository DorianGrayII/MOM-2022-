namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Quaternion), HutongGames.PlayMaker.Tooltip("Inverse a quaternion")]
    public class QuaternionInverse : QuaternionBaseAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("the rotation")]
        public FsmQuaternion rotation;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the inverse of the rotation variable.")]
        public FsmQuaternion result;

        private void DoQuatInverse()
        {
            this.result.set_Value(Quaternion.Inverse(this.rotation.get_Value()));
        }

        public override void OnEnter()
        {
            this.DoQuatInverse();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
            {
                this.DoQuatInverse();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
            {
                this.DoQuatInverse();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
            {
                this.DoQuatInverse();
            }
        }

        public override void Reset()
        {
            this.rotation = null;
            this.result = null;
            base.everyFrame = true;
            base.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
        }
    }
}

