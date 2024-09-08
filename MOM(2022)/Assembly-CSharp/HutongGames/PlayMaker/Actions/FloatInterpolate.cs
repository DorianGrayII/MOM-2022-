namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Math), HutongGames.PlayMaker.Tooltip("Interpolates between 2 Float values over a specified Time.")]
    public class FloatInterpolate : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Interpolation mode: Linear or EaseInOut.")]
        public InterpolationType mode;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Interpolate from this value.")]
        public FsmFloat fromFloat;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Interpolate to this value.")]
        public FsmFloat toFloat;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Interpolate over this amount of time in seconds.")]
        public FsmFloat time;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the current value in a float variable.")]
        public FsmFloat storeResult;
        [HutongGames.PlayMaker.Tooltip("Event to send when the interpolation is finished.")]
        public FsmEvent finishEvent;
        [HutongGames.PlayMaker.Tooltip("Ignore TimeScale. Useful if the game is paused (Time scaled to 0).")]
        public bool realTime;
        private float startTime;
        private float currentTime;

        public override void OnEnter()
        {
            this.startTime = FsmTime.RealtimeSinceStartup;
            this.currentTime = 0f;
            if (this.storeResult == null)
            {
                base.Finish();
            }
            else
            {
                this.storeResult.Value = this.fromFloat.Value;
            }
        }

        public override void OnUpdate()
        {
            this.currentTime = !this.realTime ? (this.currentTime + Time.deltaTime) : (FsmTime.RealtimeSinceStartup - this.startTime);
            float t = this.currentTime / this.time.Value;
            InterpolationType mode = this.mode;
            if (mode == InterpolationType.Linear)
            {
                this.storeResult.Value = Mathf.Lerp(this.fromFloat.Value, this.toFloat.Value, t);
            }
            else if (mode == InterpolationType.EaseInOut)
            {
                this.storeResult.Value = Mathf.SmoothStep(this.fromFloat.Value, this.toFloat.Value, t);
            }
            if (t >= 1f)
            {
                if (this.finishEvent != null)
                {
                    base.Fsm.Event(this.finishEvent);
                }
                base.Finish();
            }
        }

        public override void Reset()
        {
            this.mode = InterpolationType.Linear;
            this.fromFloat = null;
            this.toFloat = null;
            this.time = 1f;
            this.storeResult = null;
            this.finishEvent = null;
            this.realTime = false;
        }
    }
}

