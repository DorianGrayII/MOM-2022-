namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Sends an Event when the specified Mouse Button is released. Optionally store the button state in a bool variable.")]
    public class GetMouseButtonUp : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The mouse button to test.")]
        public MouseButton button;
        [HutongGames.PlayMaker.Tooltip("Event to send if the mouse button is down.")]
        public FsmEvent sendEvent;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the pressed state in a Bool Variable.")]
        public FsmBool storeResult;
        [HutongGames.PlayMaker.Tooltip("Uncheck to run when entering the state.")]
        public bool inUpdateOnly;

        public void DoGetMouseButtonUp()
        {
            bool mouseButtonUp = Input.GetMouseButtonUp((int) this.button);
            if (mouseButtonUp)
            {
                base.Fsm.Event(this.sendEvent);
            }
            this.storeResult.Value = mouseButtonUp;
        }

        public override void OnEnter()
        {
            if (!this.inUpdateOnly)
            {
                this.DoGetMouseButtonUp();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetMouseButtonUp();
        }

        public override void Reset()
        {
            this.button = MouseButton.Left;
            this.sendEvent = null;
            this.storeResult = null;
            this.inUpdateOnly = true;
        }
    }
}

