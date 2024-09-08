namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Quaternion)]
    [Tooltip("Get the quaternion from a quaternion multiplied by a quaternion.")]
    public class GetQuaternionMultipliedByQuaternion : QuaternionBaseAction
    {
        [RequiredField]
        [Tooltip("The first quaternion to multiply")]
        public FsmQuaternion quaternionA;

        [RequiredField]
        [Tooltip("The second quaternion to multiply")]
        public FsmQuaternion quaternionB;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The resulting quaternion")]
        public FsmQuaternion result;

        public override void Reset()
        {
            this.quaternionA = null;
            this.quaternionB = null;
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
            this.result.Value = this.quaternionA.Value * this.quaternionB.Value;
        }
    }
}
