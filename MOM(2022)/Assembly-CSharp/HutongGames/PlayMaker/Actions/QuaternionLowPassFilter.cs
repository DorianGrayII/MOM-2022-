using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Quaternion)]
    [Tooltip("Use a low pass filter to reduce the influence of sudden changes in a quaternion Variable.")]
    public class QuaternionLowPassFilter : QuaternionBaseAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("quaternion Variable to filter. Should generally come from some constantly updated input")]
        public FsmQuaternion quaternionVariable;

        [Tooltip("Determines how much influence new changes have. E.g., 0.1 keeps 10 percent of the unfiltered quaternion and 90 percent of the previously filtered value.")]
        public FsmFloat filteringFactor;

        private Quaternion filteredQuaternion;

        public override void Reset()
        {
            this.quaternionVariable = null;
            this.filteringFactor = 0.1f;
            base.everyFrame = true;
            base.everyFrameOption = everyFrameOptions.Update;
        }

        public override void OnEnter()
        {
            this.filteredQuaternion = new Quaternion(this.quaternionVariable.Value.x, this.quaternionVariable.Value.y, this.quaternionVariable.Value.z, this.quaternionVariable.Value.w);
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.Update)
            {
                this.DoQuatLowPassFilter();
            }
        }

        public override void OnLateUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.LateUpdate)
            {
                this.DoQuatLowPassFilter();
            }
        }

        public override void OnFixedUpdate()
        {
            if (base.everyFrameOption == everyFrameOptions.FixedUpdate)
            {
                this.DoQuatLowPassFilter();
            }
        }

        private void DoQuatLowPassFilter()
        {
            this.filteredQuaternion.x = this.quaternionVariable.Value.x * this.filteringFactor.Value + this.filteredQuaternion.x * (1f - this.filteringFactor.Value);
            this.filteredQuaternion.y = this.quaternionVariable.Value.y * this.filteringFactor.Value + this.filteredQuaternion.y * (1f - this.filteringFactor.Value);
            this.filteredQuaternion.z = this.quaternionVariable.Value.z * this.filteringFactor.Value + this.filteredQuaternion.z * (1f - this.filteringFactor.Value);
            this.filteredQuaternion.w = this.quaternionVariable.Value.w * this.filteringFactor.Value + this.filteredQuaternion.w * (1f - this.filteringFactor.Value);
            this.quaternionVariable.Value = new Quaternion(this.filteredQuaternion.x, this.filteredQuaternion.y, this.filteredQuaternion.z, this.filteredQuaternion.w);
        }
    }
}
