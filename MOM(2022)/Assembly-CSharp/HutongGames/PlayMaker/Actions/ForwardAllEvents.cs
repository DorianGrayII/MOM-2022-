namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.StateMachine), Tooltip("Forwards all event received by this FSM to another target. Optionally specify a list of events to ignore.")]
    public class ForwardAllEvents : FsmStateAction
    {
        [Tooltip("Forward to this target.")]
        public FsmEventTarget forwardTo;
        [Tooltip("Don't forward these events.")]
        public FsmEvent[] exceptThese;
        [Tooltip("Should this action eat the events or pass them on.")]
        public bool eatEvents;

        public override bool Event(FsmEvent fsmEvent)
        {
            if (this.exceptThese != null)
            {
                FsmEvent[] exceptThese = this.exceptThese;
                for (int i = 0; i < exceptThese.Length; i++)
                {
                    if (ReferenceEquals(exceptThese[i], fsmEvent))
                    {
                        return false;
                    }
                }
            }
            base.Fsm.Event(this.forwardTo, fsmEvent);
            return this.eatEvents;
        }

        public override void Reset()
        {
            FsmEventTarget target1 = new FsmEventTarget();
            target1.target = FsmEventTarget.EventTarget.FSMComponent;
            this.forwardTo = target1;
            this.exceptThese = new FsmEvent[] { FsmEvent.Finished };
            this.eatEvents = true;
        }
    }
}

