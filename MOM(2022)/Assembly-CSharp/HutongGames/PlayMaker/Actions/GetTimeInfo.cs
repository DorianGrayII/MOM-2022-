namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Time), HutongGames.PlayMaker.Tooltip("Gets various useful Time measurements.")]
    public class GetTimeInfo : FsmStateAction
    {
        public TimeInfo getInfo;
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmFloat storeValue;
        public bool everyFrame;

        private void DoGetTimeInfo()
        {
            switch (this.getInfo)
            {
                case TimeInfo.DeltaTime:
                    this.storeValue.Value = Time.deltaTime;
                    return;

                case TimeInfo.TimeScale:
                    this.storeValue.Value = Time.timeScale;
                    return;

                case TimeInfo.SmoothDeltaTime:
                    this.storeValue.Value = Time.smoothDeltaTime;
                    return;

                case TimeInfo.TimeInCurrentState:
                    this.storeValue.Value = base.State.StateTime;
                    return;

                case TimeInfo.TimeSinceStartup:
                    this.storeValue.Value = Time.time;
                    return;

                case TimeInfo.TimeSinceLevelLoad:
                    this.storeValue.Value = Time.timeSinceLevelLoad;
                    return;

                case TimeInfo.RealTimeSinceStartup:
                    this.storeValue.Value = FsmTime.RealtimeSinceStartup;
                    return;

                case TimeInfo.RealTimeInCurrentState:
                    this.storeValue.Value = FsmTime.RealtimeSinceStartup - base.State.RealStartTime;
                    return;
            }
            this.storeValue.Value = 0f;
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

        public override void Reset()
        {
            this.getInfo = TimeInfo.TimeSinceLevelLoad;
            this.storeValue = null;
            this.everyFrame = false;
        }

        public enum TimeInfo
        {
            DeltaTime,
            TimeScale,
            SmoothDeltaTime,
            TimeInCurrentState,
            TimeSinceStartup,
            TimeSinceLevelLoad,
            RealTimeSinceStartup,
            RealTimeInCurrentState
        }
    }
}

