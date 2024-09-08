namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Quaternion)]
    [Tooltip("Get the vector3 from a quaternion multiplied by a vector.")]
    public class GetQuaternionMultipliedByVector : QuaternionBaseAction
    {
        [RequiredField]
        [Tooltip("The quaternion to multiply")]
        public FsmQuaternion quaternion;

        [RequiredField]
        [Tooltip("The vector3 to multiply")]
        public FsmVector3 vector3;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The resulting vector3")]
        public FsmVector3 result;

        public override void Reset()
        {
            this.quaternion = null;
            this.vector3 = null;
            this.result = null;
            base.everyFrame = false;
            base.everyFrameOption = everyFrameOptions.Update;
        }

        public override void OnEnter()
        {
            this.DoQuatMult();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.Update)
            {
                this.DoQuatMult();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.LateUpdate)
            {
                this.DoQuatMult();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.FixedUpdate)
            {
                this.DoQuatMult();
            }
        }

        private void DoQuatMult()
        {
            this.result.Value = this.quaternion.Value * this.vector3.Value;
        }
    }
}
