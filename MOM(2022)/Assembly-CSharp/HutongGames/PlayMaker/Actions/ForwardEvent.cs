namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.StateMachine), Tooltip("Forward an event received by this FSM to another target.")]
    public class ForwardEvent : FsmStateAction
    {
        [Tooltip("Forward to this target.")]
        public FsmEventTarget forwardTo;
        [Tooltip("The events to forward.")]
        public FsmEvent[] eventsToForward;
        [Tooltip("Should this action eat the events or pass them on.")]
        public bool eatEvents;

        public override bool Event(FsmEvent fsmEvent)
        {
            if (this.eventsToForward != null)
            {
                FsmEvent[] eventsToForward = this.eventsToForward;
                for (int i = 0; i < eventsToForward.Length; i++)
                {
                    if (ReferenceEquals(eventsToForward[i], fsmEvent))
                    {
                        base.Fsm.Event(this.forwardTo, fsmEvent);
                        return this.eatEvents;
                    }
                }
            }
            return false;
        }

        public override void Reset()
        {
            FsmEventTarget target1 = new FsmEventTarget();
            target1.target = FsmEventTarget.EventTarget.FSMComponent;
            this.forwardTo = target1;
            this.eventsToForward = null;
            this.eatEvents = true;
        }
    }
}

