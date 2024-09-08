namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Vector2), HutongGames.PlayMaker.Tooltip("Interpolates between 2 Vector2 values over a specified Time.")]
    public class Vector2Interpolate : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The interpolation type")]
        public InterpolationType mode;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The vector to interpolate from")]
        public FsmVector2 fromVector;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The vector to interpolate to")]
        public FsmVector2 toVector;
        [RequiredField, HutongGames.PlayMaker.Tooltip("the interpolate time")]
        public FsmFloat time;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("the interpolated result")]
        public FsmVector2 storeResult;
        [HutongGames.PlayMaker.Tooltip("This event is fired when the interpolation is done.")]
        public FsmEvent finishEvent;
        [HutongGames.PlayMaker.Tooltip("Ignore TimeScale")]
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
                this.storeResult.set_Value(this.fromVector.get_Value());
            }
        }

        public override void OnUpdate()
        {
            this.currentTime = !this.realTime ? (this.currentTime + Time.deltaTime) : (FsmTime.RealtimeSinceStartup - this.startTime);
            float t = this.currentTime / this.time.Value;
            InterpolationType mode = this.mode;
            if ((mode != InterpolationType.Linear) && (mode == InterpolationType.EaseInOut))
            {
                t = Mathf.SmoothStep(0f, 1f, t);
            }
            this.storeResult.set_Value(Vector2.Lerp(this.fromVector.get_Value(), this.toVector.get_Value(), t));
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
            FsmVector2 vector1 = new FsmVector2();
            vector1.UseVariable = true;
            this.fromVector = vector1;
            FsmVector2 vector2 = new FsmVector2();
            vector2.UseVariable = true;
            this.toVector = vector2;
            this.time = 1f;
            this.storeResult = null;
            this.finishEvent = null;
            this.realTime = false;
        }
    }
}

