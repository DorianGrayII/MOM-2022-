namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.StateMachine), HutongGames.PlayMaker.Tooltip("Gets the sender of the last event.")]
    public class GetEventSentBy : FsmStateAction
    {
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the GameObject that sent the event.")]
        public FsmGameObject sentByGameObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the name of the GameObject that sent the event.")]
        public FsmString gameObjectName;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the name of the FSM that sent the event.")]
        public FsmString fsmName;

        public override void OnEnter()
        {
            if (Fsm.EventData.SentByGameObject != null)
            {
                this.sentByGameObject.set_Value(Fsm.EventData.SentByGameObject);
            }
            else if (Fsm.EventData.SentByFsm != null)
            {
                this.sentByGameObject.set_Value(Fsm.EventData.SentByFsm.get_GameObject());
                this.fsmName.Value = Fsm.EventData.SentByFsm.Name;
            }
            else
            {
                this.sentByGameObject.set_Value((GameObject) null);
                this.fsmName.Value = "";
            }
            if (this.sentByGameObject.get_Value() != null)
            {
                this.gameObjectName.Value = this.sentByGameObject.get_Value().name;
            }
            base.Finish();
        }

        public override void Reset()
        {
            this.sentByGameObject = null;
            this.gameObjectName = null;
            this.fsmName = null;
        }
    }
}

