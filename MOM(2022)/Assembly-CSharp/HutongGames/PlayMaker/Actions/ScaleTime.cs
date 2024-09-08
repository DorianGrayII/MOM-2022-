using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Time)]
    [Tooltip("Scales time: 1 = normal, 0.5 = half speed, 2 = double speed.")]
    public class ScaleTime : FsmStateAction
    {
        [RequiredField]
        [HasFloatSlider(0f, 4f)]
        [Tooltip("Scales time: 1 = normal, 0.5 = half speed, 2 = double speed.")]
        public FsmFloat timeScale;

        [Tooltip("Adjust the fixed physics time step to match the time scale.")]
        public FsmBool adjustFixedDeltaTime;

        [Tooltip("Repeat every frame. Useful when animating the value.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.timeScale = 1f;
            this.adjustFixedDeltaTime = true;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoTimeScale();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoTimeScale();
        }

        private void DoTimeScale()
        {
            Time.timeScale = this.timeScale.Value;
            if (this.adjustFixedDeltaTime.Value)
            {
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
            }
        }
    }
}
