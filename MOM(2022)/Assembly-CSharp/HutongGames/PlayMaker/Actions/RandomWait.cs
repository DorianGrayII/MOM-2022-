namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Time), HutongGames.PlayMaker.Tooltip("Delays a State from finishing by a random time. NOTE: Other actions continue, but FINISHED can't happen before Time.")]
    public class RandomWait : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("Minimum amount of time to wait.")]
        public FsmFloat min;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Maximum amount of time to wait.")]
        public FsmFloat max;
        [HutongGames.PlayMaker.Tooltip("Event to send when timer is finished.")]
        public FsmEvent finishEvent;
        [HutongGames.PlayMaker.Tooltip("Ignore time scale.")]
        public bool realTime;
        private float startTime;
        private float timer;
        private float time;

        public override void OnEnter()
        {
            this.time = UnityEngine.Random.Range(this.min.Value, this.max.Value);
            if (this.time <= 0f)
            {
                base.Fsm.Event(this.finishEvent);
                base.Finish();
            }
            else
            {
                this.startTime = FsmTime.RealtimeSinceStartup;
                this.timer = 0f;
            }
        }

        public override void OnUpdate()
        {
            this.timer = !this.realTime ? (this.timer + Time.deltaTime) : (FsmTime.RealtimeSinceStartup - this.startTime);
            if (this.timer >= this.time)
            {
                base.Finish();
                if (this.finishEvent != null)
                {
                    base.Fsm.Event(this.finishEvent);
                }
            }
        }

        public override void Reset()
        {
            this.min = 0f;
            this.max = 1f;
            this.finishEvent = null;
            this.realTime = false;
        }
    }
}

