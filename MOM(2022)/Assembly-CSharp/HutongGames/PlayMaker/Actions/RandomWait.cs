using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Time)]
    [Tooltip("Delays a State from finishing by a random time. NOTE: Other actions continue, but FINISHED can't happen before Time.")]
    public class RandomWait : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Minimum amount of time to wait.")]
        public FsmFloat min;

        [RequiredField]
        [Tooltip("Maximum amount of time to wait.")]
        public FsmFloat max;

        [Tooltip("Event to send when timer is finished.")]
        public FsmEvent finishEvent;

        [Tooltip("Ignore time scale.")]
        public bool realTime;

        private float startTime;

        private float timer;

        private float time;

        public override void Reset()
        {
            this.min = 0f;
            this.max = 1f;
            this.finishEvent = null;
            this.realTime = false;
        }

        public override void OnEnter()
        {
            this.time = Random.Range(this.min.Value, this.max.Value);
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
            if (this.realTime)
            {
                this.timer = FsmTime.RealtimeSinceStartup - this.startTime;
            }
            else
            {
                this.timer += Time.deltaTime;
            }
            if (this.timer >= this.time)
            {
                base.Finish();
                if (this.finishEvent != null)
                {
                    base.Fsm.Event(this.finishEvent);
                }
            }
        }
    }
}
