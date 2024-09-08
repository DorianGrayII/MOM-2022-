namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Quaternion), HutongGames.PlayMaker.Tooltip("Interpolates between from and to by t and normalizes the result afterwards.")]
    public class QuaternionLerp : QuaternionBaseAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("From Quaternion.")]
        public FsmQuaternion fromQuaternion;
        [RequiredField, HutongGames.PlayMaker.Tooltip("To Quaternion.")]
        public FsmQuaternion toQuaternion;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Interpolate between fromQuaternion and toQuaternion by this amount. Value is clamped to 0-1 range. 0 = fromQuaternion; 1 = toQuaternion; 0.5 = half way between."), HasFloatSlider(0f, 1f)]
        public FsmFloat amount;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the result in this quaternion variable.")]
        public FsmQuaternion storeResult;

        private void DoQuatLerp()
        {
            this.storeResult.set_Value(Quaternion.Lerp(this.fromQuaternion.get_Value(), this.toQuaternion.get_Value(), this.amount.Value));
        }

        public override void OnEnter()
        {
            this.DoQuatLerp();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
            {
                this.DoQuatLerp();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
            {
                this.DoQuatLerp();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
            {
                this.DoQuatLerp();
            }
        }

        public override void Reset()
        {
            FsmQuaternion quaternion1 = new FsmQuaternion();
            quaternion1.UseVariable = true;
            this.fromQuaternion = quaternion1;
            FsmQuaternion quaternion2 = new FsmQuaternion();
            quaternion2.UseVariable = true;
            this.toQuaternion = quaternion2;
            this.amount = 0.5f;
            this.storeResult = null;
            base.everyFrame = true;
            base.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
        }
    }
}

