namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Vector3), HutongGames.PlayMaker.Tooltip("Interpolates between 2 Vector3 values over a specified Time.")]
    public class Vector3Interpolate : FsmStateAction
    {
        public InterpolationType mode;
        [RequiredField]
        public FsmVector3 fromVector;
        [RequiredField]
        public FsmVector3 toVector;
        [RequiredField]
        public FsmFloat time;
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmVector3 storeResult;
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
            this.storeResult.set_Value(Vector3.Lerp(this.fromVector.get_Value(), this.toVector.get_Value(), t));
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
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.fromVector = vector1;
            FsmVector3 vector2 = new FsmVector3();
            vector2.UseVariable = true;
            this.toVector = vector2;
            this.time = 1f;
            this.storeResult = null;
            this.finishEvent = null;
            this.realTime = false;
        }
    }
}

