using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Time)]
    [Tooltip("Gets various useful Time measurements.")]
    public class GetTimeInfo : FsmStateAction
    {
        public enum TimeInfo
        {
            DeltaTime = 0,
            TimeScale = 1,
            SmoothDeltaTime = 2,
            TimeInCurrentState = 3,
            TimeSinceStartup = 4,
            TimeSinceLevelLoad = 5,
            RealTimeSinceStartup = 6,
            RealTimeInCurrentState = 7
        }

        public TimeInfo getInfo;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmFloat storeValue;

        public bool everyFrame;

        public override void Reset()
        {
            this.getInfo = TimeInfo.TimeSinceLevelLoad;
            this.storeValue = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoGetTimeInfo();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetTimeInfo();
        }

        private void DoGetTimeInfo()
        {
            switch (this.getInfo)
            {
            case TimeInfo.DeltaTime:
                this.storeValue.Value = Time.deltaTime;
                break;
            case TimeInfo.TimeScale:
                this.storeValue.Value = Time.timeScale;
                break;
            case TimeInfo.SmoothDeltaTime:
                this.storeValue.Value = Time.smoothDeltaTime;
                break;
            case TimeInfo.TimeInCurrentState:
                this.storeValue.Value = base.State.StateTime;
                break;
            case TimeInfo.TimeSinceStartup:
                this.storeValue.Value = Time.time;
                break;
            case TimeInfo.TimeSinceLevelLoad:
                this.storeValue.Value = Time.timeSinceLevelLoad;
                break;
            case TimeInfo.RealTimeSinceStartup:
                this.storeValue.Value = FsmTime.RealtimeSinceStartup;
                break;
            case TimeInfo.RealTimeInCurrentState:
                this.storeValue.Value = FsmTime.RealtimeSinceStartup - base.State.RealStartTime;
                break;
            default:
                this.storeValue.Value = 0f;
                break;
            }
        }
    }
}
