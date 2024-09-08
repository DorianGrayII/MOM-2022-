﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Physics), Tooltip("Gets info on the last Trigger event and store in variables.")]
    public class GetTriggerInfo : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        public FsmGameObject gameObjectHit;
        [UIHint(UIHint.Variable), Tooltip("Useful for triggering different effects. Audio, particles...")]
        public FsmString physicsMaterialName;

        public override void OnEnter()
        {
            this.StoreTriggerInfo();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObjectHit = null;
            this.physicsMaterialName = null;
        }

        private void StoreTriggerInfo()
        {
            if (base.Fsm.get_TriggerCollider() != null)
            {
                this.gameObjectHit.set_Value(base.Fsm.get_TriggerCollider().gameObject);
                this.physicsMaterialName.Value = base.Fsm.get_TriggerCollider().material.name;
            }
        }
    }
}

