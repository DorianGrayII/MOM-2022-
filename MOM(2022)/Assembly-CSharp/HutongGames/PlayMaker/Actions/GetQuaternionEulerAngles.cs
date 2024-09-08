namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Quaternion), Tooltip("Gets a quaternion as euler angles.")]
    public class GetQuaternionEulerAngles : QuaternionBaseAction
    {
        [RequiredField, Tooltip("The rotation")]
        public FsmQuaternion quaternion;
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The euler angles of the quaternion.")]
        public FsmVector3 eulerAngles;

        private void GetQuatEuler()
        {
            this.eulerAngles.set_Value(this.quaternion.get_Value().eulerAngles);
        }

        public override void OnEnter()
        {
            this.GetQuatEuler();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
            {
                this.GetQuatEuler();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
            {
                this.GetQuatEuler();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
            {
                this.GetQuatEuler();
            }
        }

        public override void Reset()
        {
            this.quaternion = null;
            this.eulerAngles = null;
            base.everyFrame = true;
            base.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
        }
    }
}

