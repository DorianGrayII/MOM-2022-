namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    public abstract class QuaternionBaseAction : FsmStateAction
    {
        [Tooltip("Repeat every frame. Useful if any of the values are changing.")]
        public bool everyFrame;
        [Tooltip("Defines how to perform the action when 'every Frame' is enabled.")]
        public everyFrameOptions everyFrameOption;

        protected QuaternionBaseAction()
        {
        }

        public override void Awake()
        {
            if (this.everyFrame)
            {
                everyFrameOptions everyFrameOption = this.everyFrameOption;
                if (everyFrameOption == everyFrameOptions.FixedUpdate)
                {
                    base.Fsm.HandleFixedUpdate = true;
                }
                else if (everyFrameOption == everyFrameOptions.LateUpdate)
                {
                    base.Fsm.HandleLateUpdate = true;
                }
            }
        }

        public enum everyFrameOptions
        {
            Update,
            FixedUpdate,
            LateUpdate
        }
    }
}

