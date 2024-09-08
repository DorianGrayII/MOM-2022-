using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Array)]
    [Tooltip("Iterate through the items in an Array and run an FSM on each item. NOTE: The FSM has to Finish before being run on the next item.")]
    public class ArrayForEach : RunFSMAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Array to iterate through.")]
        public FsmArray array;

        [HideTypeFilter]
        [MatchElementType("array")]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the item in a variable")]
        public FsmVar storeItem;

        [ActionSection("Run FSM")]
        public FsmTemplateControl fsmTemplateControl = new FsmTemplateControl();

        [Tooltip("Event to send after iterating through all items in the Array.")]
        public FsmEvent finishEvent;

        private int currentIndex;

        public override void Reset()
        {
            this.array = null;
            this.fsmTemplateControl = new FsmTemplateControl();
            base.runFsm = null;
        }

        public override void Awake()
        {
            if (this.array != null && this.fsmTemplateControl.fsmTemplate != null && Application.isPlaying)
            {
                base.runFsm = base.Fsm.CreateSubFsm(this.fsmTemplateControl);
            }
        }

        public override void OnEnter()
        {
            if (this.array == null || base.runFsm == null)
            {
                base.Finish();
                return;
            }
            this.currentIndex = 0;
            this.StartFsm();
        }

        public override void OnUpdate()
        {
            base.runFsm.Update();
            if (base.runFsm.Finished)
            {
                this.StartNextFsm();
            }
        }

        public override void OnFixedUpdate()
        {
            base.runFsm.FixedUpdate();
            if (base.runFsm.Finished)
            {
                this.StartNextFsm();
            }
        }

        public override void OnLateUpdate()
        {
            base.runFsm.LateUpdate();
            if (base.runFsm.Finished)
            {
                this.StartNextFsm();
            }
        }

        private void StartNextFsm()
        {
            this.currentIndex++;
            this.StartFsm();
        }

        private void StartFsm()
        {
            while (this.currentIndex < this.array.Length)
            {
                this.DoStartFsm();
                if (!base.runFsm.Finished)
                {
                    return;
                }
                this.currentIndex++;
            }
            base.Fsm.Event(this.finishEvent);
            base.Finish();
        }

        private void DoStartFsm()
        {
            this.storeItem.SetValue(this.array.Values[this.currentIndex]);
            this.fsmTemplateControl.UpdateValues();
            this.fsmTemplateControl.ApplyOverrides(base.runFsm);
            base.runFsm.OnEnable();
            if (!base.runFsm.Started)
            {
                base.runFsm.Start();
            }
        }

        protected override void CheckIfFinished()
        {
        }
    }
}
