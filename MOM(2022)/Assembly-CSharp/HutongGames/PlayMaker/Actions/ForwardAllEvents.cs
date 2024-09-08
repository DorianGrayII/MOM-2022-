namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [Tooltip("Forwards all event received by this FSM to another target. Optionally specify a list of events to ignore.")]
    public class ForwardAllEvents : FsmStateAction
    {
        [Tooltip("Forward to this target.")]
        public FsmEventTarget forwardTo;

        [Tooltip("Don't forward these events.")]
        public FsmEvent[] exceptThese;

        [Tooltip("Should this action eat the events or pass them on.")]
        public bool eatEvents;

        public override void Reset()
        {
            this.forwardTo = new FsmEventTarget
            {
                target = FsmEventTarget.EventTarget.FSMComponent
            };
            this.exceptThese = new FsmEvent[1] { FsmEvent.Finished };
            this.eatEvents = true;
        }

        public override bool Event(FsmEvent fsmEvent)
        {
            if (this.exceptThese != null)
            {
                FsmEvent[] array = this.exceptThese;
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] == fsmEvent)
                    {
                        return false;
                    }
                }
            }
            base.Fsm.Event(this.forwardTo, fsmEvent);
            return this.eatEvents;
        }
    }
}
