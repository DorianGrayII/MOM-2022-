namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Logic), HutongGames.PlayMaker.Tooltip("Tests if the value of a GameObject variable changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
    public class GameObjectChanged : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The GameObject variable to watch for a change.")]
        public FsmGameObject gameObjectVariable;
        [HutongGames.PlayMaker.Tooltip("Event to send if the variable changes.")]
        public FsmEvent changedEvent;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Set to True if the variable changes.")]
        public FsmBool storeResult;
        private GameObject previousValue;

        public override void OnEnter()
        {
            if (this.gameObjectVariable.IsNone)
            {
                base.Finish();
            }
            else
            {
                this.previousValue = this.gameObjectVariable.get_Value();
            }
        }

        public override void OnUpdate()
        {
            this.storeResult.Value = false;
            if (this.gameObjectVariable.get_Value() != this.previousValue)
            {
                this.storeResult.Value = true;
                base.Fsm.Event(this.changedEvent);
            }
        }

        public override void Reset()
        {
            this.gameObjectVariable = null;
            this.changedEvent = null;
            this.storeResult = null;
        }
    }
}

