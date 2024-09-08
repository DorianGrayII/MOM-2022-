using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Quaternion)]
    [Tooltip("Interpolates between from and to by t and normalizes the result afterwards.")]
    public class QuaternionLerp : QuaternionBaseAction
    {
        [RequiredField]
        [Tooltip("From Quaternion.")]
        public FsmQuaternion fromQuaternion;

        [RequiredField]
        [Tooltip("To Quaternion.")]
        public FsmQuaternion toQuaternion;

        [RequiredField]
        [Tooltip("Interpolate between fromQuaternion and toQuaternion by this amount. Value is clamped to 0-1 range. 0 = fromQuaternion; 1 = toQuaternion; 0.5 = half way between.")]
        [HasFloatSlider(0f, 1f)]
        public FsmFloat amount;

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
            this.amount = 0.5f;
            this.storeResult = null;
            base.everyFrame = true;
            base.everyFrameOption = everyFrameOptions.Update;
        }

        public override void OnEnter()
        {
            this.DoQuatLerp();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.Update)
            {
                this.DoQuatLerp();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.LateUpdate)
            {
                this.DoQuatLerp();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.FixedUpdate)
            {
                this.DoQuatLerp();
            }
        }

        private void DoQuatLerp()
        {
            this.storeResult.Value = Quaternion.Lerp(this.fromQuaternion.Value, this.toQuaternion.Value, this.amount.Value);
        }
    }
}
