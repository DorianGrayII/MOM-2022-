namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Quaternion)]
    [Tooltip("Gets a quaternion as euler angles.")]
    public class GetQuaternionEulerAngles : QuaternionBaseAction
    {
        [RequiredField]
        [Tooltip("The rotation")]
        public FsmQuaternion quaternion;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The euler angles of the quaternion.")]
        public FsmVector3 eulerAngles;

        public override void Reset()
        {
            this.quaternion = null;
            this.eulerAngles = null;
            base.everyFrame = true;
            base.everyFrameOption = everyFrameOptions.Update;
        }

        public override void OnEnter()
        {
            this.GetQuatEuler();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.Update)
            {
                this.GetQuatEuler();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.LateUpdate)
            {
                this.GetQuatEuler();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.FixedUpdate)
            {
                this.GetQuatEuler();
            }
        }

        private void GetQuatEuler()
        {
            this.eulerAngles.Value = this.quaternion.Value.eulerAngles;
        }
    }
}
