namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Device), HutongGames.PlayMaker.Tooltip("Sends an Event when the mobile device is shaken.")]
    public class DeviceShakeEvent : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("Amount of acceleration required to trigger the event. Higher numbers require a harder shake.")]
        public FsmFloat shakeThreshold;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Event to send when Shake Threshold is exceeded.")]
        public FsmEvent sendEvent;

        public override void OnUpdate()
        {
            if (Input.acceleration.sqrMagnitude > (this.shakeThreshold.Value * this.shakeThreshold.Value))
            {
                base.Fsm.Event(this.sendEvent);
            }
        }

        public override void Reset()
        {
            this.shakeThreshold = 3f;
            this.sendEvent = null;
        }
    }
}

