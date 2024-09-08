using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Vector2)]
    [Tooltip("Interpolates between 2 Vector2 values over a specified Time.")]
    public class Vector2Interpolate : FsmStateAction
    {
        [Tooltip("The interpolation type")]
        public InterpolationType mode;

        [RequiredField]
        [Tooltip("The vector to interpolate from")]
        public FsmVector2 fromVector;

        [RequiredField]
        [Tooltip("The vector to interpolate to")]
        public FsmVector2 toVector;

        [RequiredField]
        [Tooltip("the interpolate time")]
        public FsmFloat time;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("the interpolated result")]
        public FsmVector2 storeResult;

        [Tooltip("This event is fired when the interpolation is done.")]
        public FsmEvent finishEvent;

        [Tooltip("Ignore TimeScale")]
        public bool realTime;

        private float startTime;

        private float currentTime;

        public override void Reset()
        {
            this.mode = InterpolationType.Linear;
            this.fromVector = new FsmVector2
            {
                UseVariable = true
            };
            this.toVector = new FsmVector2
            {
                UseVariable = true
            };
            this.time = 1f;
            this.storeResult = null;
            this.finishEvent = null;
            this.realTime = false;
        }

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
                this.storeResult.Value = this.fromVector.Value;
            }
        }

        public override void OnUpdate()
        {
            if (this.realTime)
            {
                this.currentTime = FsmTime.RealtimeSinceStartup - this.startTime;
            }
            else
            {
                this.currentTime += Time.deltaTime;
            }
            float num = this.currentTime / this.time.Value;
            InterpolationType interpolationType = this.mode;
            if (interpolationType != 0 && interpolationType == InterpolationType.EaseInOut)
            {
                num = Mathf.SmoothStep(0f, 1f, num);
            }
            this.storeResult.Value = Vector2.Lerp(this.fromVector.Value, this.toVector.Value, num);
            if (num >= 1f)
            {
                if (this.finishEvent != null)
                {
                    base.Fsm.Event(this.finishEvent);
                }
                base.Finish();
            }
        }
    }
}
