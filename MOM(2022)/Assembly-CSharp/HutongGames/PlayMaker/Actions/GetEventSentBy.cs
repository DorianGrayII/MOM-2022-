namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [Tooltip("Gets the sender of the last event.")]
    public class GetEventSentBy : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the GameObject that sent the event.")]
        public FsmGameObject sentByGameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the name of the GameObject that sent the event.")]
        public FsmString gameObjectName;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the name of the FSM that sent the event.")]
        public FsmString fsmName;

        public override void Reset()
        {
            this.sentByGameObject = null;
            this.gameObjectName = null;
            this.fsmName = null;
        }

        public override void OnEnter()
        {
            if (Fsm.EventData.SentByGameObject != null)
            {
                this.sentByGameObject.Value = Fsm.EventData.SentByGameObject;
            }
            else if (Fsm.EventData.SentByFsm != null)
            {
                this.sentByGameObject.Value = Fsm.EventData.SentByFsm.GameObject;
                this.fsmName.Value = Fsm.EventData.SentByFsm.Name;
            }
            else
            {
                this.sentByGameObject.Value = null;
                this.fsmName.Value = "";
            }
            if (this.sentByGameObject.Value != null)
            {
                this.gameObjectName.Value = this.sentByGameObject.Value.name;
            }
            base.Finish();
        }
    }
}
