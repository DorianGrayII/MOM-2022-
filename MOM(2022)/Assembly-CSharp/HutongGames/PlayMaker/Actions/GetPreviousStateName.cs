﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.StateMachine), Tooltip("Gets the name of the previously active state and stores it in a String Variable.")]
    public class GetPreviousStateName : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        public FsmString storeName;

        public override void OnEnter()
        {
            this.storeName.Value = base.Fsm.PreviousActiveState?.Name;
            base.Finish();
        }

        public override void Reset()
        {
            this.storeName = null;
        }
    }
}

