using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [Tooltip("Creates an FSM from a saved FSM Template.")]
    public class RunFSM : RunFSMAction
    {
        public FsmTemplateControl fsmTemplateControl = new FsmTemplateControl();

        [UIHint(UIHint.Variable)]
        public FsmInt storeID;

        [Tooltip("Event to send when the FSM has finished (usually because it ran a Finish FSM action).")]
        public FsmEvent finishEvent;

        public override void Reset()
        {
            this.fsmTemplateControl = new FsmTemplateControl();
            this.storeID = null;
            base.runFsm = null;
        }

        public override void Awake()
        {
            if (this.fsmTemplateControl.fsmTemplate != null && Application.isPlaying)
            {
                base.runFsm = base.Fsm.CreateSubFsm(this.fsmTemplateControl);
            }
        }

        public override void OnEnter()
        {
            if (base.runFsm == null)
            {
                base.Finish();
                return;
            }
            this.fsmTemplateControl.UpdateValues();
            this.fsmTemplateControl.ApplyOverrides(base.runFsm);
            base.runFsm.OnEnable();
            if (!base.runFsm.Started)
            {
                base.runFsm.Start();
            }
            this.storeID.Value = this.fsmTemplateControl.ID;
            this.CheckIfFinished();
        }

        protected override void CheckIfFinished()
        {
            if (base.runFsm == null || base.runFsm.Finished)
            {
                base.Finish();
                base.Fsm.Event(this.finishEvent);
            }
        }
    }
}
