namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [Obsolete("This action is obsolete; use Send Event with Event Target instead."), ActionCategory(ActionCategory.StateMachine), HutongGames.PlayMaker.Tooltip("Sends an Event to another Fsm after an optional delay. Specify an Fsm Name or use the first Fsm on the object.")]
    public class SendEventToFsm : FsmStateAction
    {
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.FsmName), HutongGames.PlayMaker.Tooltip("Optional name of Fsm on Game Object")]
        public FsmString fsmName;
        [RequiredField, UIHint(UIHint.FsmEvent)]
        public FsmString sendEvent;
        [HasFloatSlider(0f, 10f)]
        public FsmFloat delay;
        private bool requireReceiver;
        private GameObject go;
        private DelayedEvent delayedEvent;

        public override void OnEnter()
        {
            this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (this.go == null)
            {
                base.Finish();
            }
            else
            {
                PlayMakerFSM gameObjectFsm = ActionHelpers.GetGameObjectFsm(this.go, this.fsmName.Value);
                if (gameObjectFsm == null)
                {
                    if (this.requireReceiver)
                    {
                        base.LogError("GameObject doesn't have FsmComponent: " + this.go.name + " " + this.fsmName.Value);
                    }
                }
                else if (this.delay.Value >= 0.001)
                {
                    this.delayedEvent = gameObjectFsm.Fsm.DelayedEvent(FsmEvent.GetFsmEvent(this.sendEvent.Value), this.delay.Value);
                }
                else
                {
                    gameObjectFsm.Fsm.Event(this.sendEvent.Value);
                    base.Finish();
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

        public override void Reset()
        {
            this.gameObject = null;
            this.fsmName = null;
            this.sendEvent = null;
            this.delay = null;
            this.requireReceiver = false;
        }
    }
}

