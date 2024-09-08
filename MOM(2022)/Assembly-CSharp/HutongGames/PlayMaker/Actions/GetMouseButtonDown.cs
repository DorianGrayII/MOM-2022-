namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Sends an Event when the specified Mouse Button is pressed. Optionally store the button state in a bool variable.")]
    public class GetMouseButtonDown : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The mouse button to test.")]
        public MouseButton button;
        [HutongGames.PlayMaker.Tooltip("Event to send if the mouse button is down.")]
        public FsmEvent sendEvent;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the button state in a Bool Variable.")]
        public FsmBool storeResult;
        [HutongGames.PlayMaker.Tooltip("Uncheck to run when entering the state.")]
        public bool inUpdateOnly;

        private void DoGetMouseButtonDown()
        {
            bool mouseButtonDown = Input.GetMouseButtonDown((int) this.button);
            if (mouseButtonDown)
            {
                base.Fsm.Event(this.sendEvent);
            }
            this.storeResult.Value = mouseButtonDown;
        }

        public override void OnEnter()
        {
            if (!this.inUpdateOnly)
            {
                this.DoGetMouseButtonDown();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetMouseButtonDown();
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

