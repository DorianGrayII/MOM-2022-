using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [Tooltip("Sends a Random State Event after an optional delay. Use this to transition to a random state from the current state.")]
    public class RandomEvent : FsmStateAction
    {
        [HasFloatSlider(0f, 10f)]
        [Tooltip("Delay before sending the event.")]
        public FsmFloat delay;

        [Tooltip("Don't repeat the same event twice in a row.")]
        public FsmBool noRepeat;

        private DelayedEvent delayedEvent;

        private int randomEventIndex;

        private int lastEventIndex = -1;

        public override void Reset()
        {
            this.delay = null;
            this.noRepeat = false;
        }

        public override void OnEnter()
        {
            if (base.State.Transitions.Length != 0)
            {
                if (this.lastEventIndex == -1)
                {
                    this.lastEventIndex = Random.Range(0, base.State.Transitions.Length);
                }
                if (this.delay.Value < 0.001f)
                {
                    base.Fsm.Event(this.GetRandomEvent());
                    base.Finish();
                }
                else
                {
                    this.delayedEvent = base.Fsm.DelayedEvent(this.GetRandomEvent(), this.delay.Value);
                }
            }
        }

        public override void OnUpdate()
        {
            if (DelayedEvent.WasSent(this.delayedEvent))
            {
                base.Finish();
            }
        }

        private FsmEvent GetRandomEvent()
        {
            do
            {
                this.randomEventIndex = Random.Range(0, base.State.Transitions.Length);
            }
            while (this.noRepeat.Value && base.State.Transitions.Length > 1 && this.randomEventIndex == this.lastEventIndex);
            this.lastEventIndex = this.randomEventIndex;
            return base.State.Transitions[this.randomEventIndex].FsmEvent;
        }
    }
}
