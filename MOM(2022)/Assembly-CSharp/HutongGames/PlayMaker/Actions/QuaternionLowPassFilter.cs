namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Quaternion), HutongGames.PlayMaker.Tooltip("Use a low pass filter to reduce the influence of sudden changes in a quaternion Variable.")]
    public class QuaternionLowPassFilter : QuaternionBaseAction
    {
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("quaternion Variable to filter. Should generally come from some constantly updated input")]
        public FsmQuaternion quaternionVariable;
        [HutongGames.PlayMaker.Tooltip("Determines how much influence new changes have. E.g., 0.1 keeps 10 percent of the unfiltered quaternion and 90 percent of the previously filtered value.")]
        public FsmFloat filteringFactor;
        private Quaternion filteredQuaternion;

        private void DoQuatLowPassFilter()
        {
            this.filteredQuaternion.x = (this.quaternionVariable.get_Value().x * this.filteringFactor.Value) + (this.filteredQuaternion.x * (1f - this.filteringFactor.Value));
            this.filteredQuaternion.y = (this.quaternionVariable.get_Value().y * this.filteringFactor.Value) + (this.filteredQuaternion.y * (1f - this.filteringFactor.Value));
            this.filteredQuaternion.z = (this.quaternionVariable.get_Value().z * this.filteringFactor.Value) + (this.filteredQuaternion.z * (1f - this.filteringFactor.Value));
            this.filteredQuaternion.w = (this.quaternionVariable.get_Value().w * this.filteringFactor.Value) + (this.filteredQuaternion.w * (1f - this.filteringFactor.Value));
            this.quaternionVariable.set_Value(new Quaternion(this.filteredQuaternion.x, this.filteredQuaternion.y, this.filteredQuaternion.z, this.filteredQuaternion.w));
        }

        public override void OnEnter()
        {
            this.filteredQuaternion = new Quaternion(this.quaternionVariable.get_Value().x, this.quaternionVariable.get_Value().y, this.quaternionVariable.get_Value().z, this.quaternionVariable.get_Value().w);
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
            {
                this.DoQuatLowPassFilter();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
            {
                this.DoQuatLowPassFilter();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
            {
                this.DoQuatLowPassFilter();
            }
        }

        public override void Reset()
        {
            this.quaternionVariable = null;
            this.filteringFactor = 0.1f;
            base.everyFrame = true;
            base.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
        }
    }
}

