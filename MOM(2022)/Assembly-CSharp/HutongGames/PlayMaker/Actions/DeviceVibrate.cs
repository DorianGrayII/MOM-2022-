namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Device), Tooltip("Causes the device to vibrate for half a second.")]
    public class DeviceVibrate : FsmStateAction
    {
        public override void OnEnter()
        {
            base.Finish();
        }

        public override void Reset()
        {
        }
    }
}

