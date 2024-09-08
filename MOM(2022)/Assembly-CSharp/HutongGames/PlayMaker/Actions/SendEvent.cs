namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.StateMachine), ActionTarget(typeof(PlayMakerFSM), "eventTarget", false), ActionTarget(typeof(GameObject), "eventTarget", false), HutongGames.PlayMaker.Tooltip("Sends an Event after an optional delay. NOTE: To send events between FSMs they must be marked as Global in the Events Browser.")]
    public class SendEvent : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The event to send. NOTE: Events must be marked Global to send between FSMs.")]
        public FsmEvent sendEvent;
        [HasFloatSlider(0f, 10f), HutongGames.PlayMaker.Tooltip("Optional delay in seconds.")]
        public FsmFloat delay;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Rarely needed, but can be useful when sending events to other FSMs.")]
        public bool everyFrame;
        private DelayedEvent delayedEvent;

        public override void OnEnter()
        {
            if (this.delay.Value >= 0.001f)
            {
                this.delayedEvent = base.Fsm.DelayedEvent(this.eventTarget, this.sendEvent, this.delay.Value);
            }
            else
            {
                base.Fsm.Event(this.eventTarget, this.sendEvent);
                if (!this.everyFrame)
                {
                    base.Finish();
                }
            }
        }

        public override void OnUpdate()
        {
            if (this.everyFrame)
            {
                base.Fsm.Event(this.eventTarget, this.sendEvent);
            }
            else if (DelayedEvent.WasSent(this.delayedEvent))
            {
                base.Finish();
            }
        }

        public override void Reset()
        {
            this.eventTarget = null;
            this.sendEvent = null;
            this.delay = null;
            this.everyFrame = false;
        }
    }
}

