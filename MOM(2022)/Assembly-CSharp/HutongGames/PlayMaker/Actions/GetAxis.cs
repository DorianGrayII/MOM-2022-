using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Input)]
    [Tooltip("Gets the value of the specified Input Axis and stores it in a Float Variable. See Unity Input Manager docs.")]
    public class GetAxis : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The name of the axis. Set in the Unity Input Manager.")]
        public FsmString axisName;

        [Tooltip("Axis values are in the range -1 to 1. Use the multiplier to set a larger range.")]
        public FsmFloat multiplier;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a float variable.")]
        public FsmFloat store;

        [Tooltip("Repeat every frame. Typically this would be set to True.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.axisName = "";
            this.multiplier = 1f;
            this.store = null;
            this.everyFrame = true;
        }

        public override void OnEnter()
        {
            this.DoGetAxis();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetAxis();
        }

        private void DoGetAxis()
        {
            if (!FsmString.IsNullOrEmpty(this.axisName))
            {
                float num = Input.GetAxis(this.axisName.Value);
                if (!this.multiplier.IsNone)
                {
                    num *= this.multiplier.Value;
                }
                this.store.Value = num;
            }
        }
    }
}
