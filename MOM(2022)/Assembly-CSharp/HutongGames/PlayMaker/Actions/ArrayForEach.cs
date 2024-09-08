namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Array), HutongGames.PlayMaker.Tooltip("Iterate through the items in an Array and run an FSM on each item. NOTE: The FSM has to Finish before being run on the next item.")]
    public class ArrayForEach : RunFSMAction
    {
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Array to iterate through.")]
        public FsmArray array;
        [HideTypeFilter, MatchElementType("array"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the item in a variable")]
        public FsmVar storeItem;
        [ActionSection("Run FSM")]
        public FsmTemplateControl fsmTemplateControl = new FsmTemplateControl();
        [HutongGames.PlayMaker.Tooltip("Event to send after iterating through all items in the Array.")]
        public FsmEvent finishEvent;
        private int currentIndex;

        public override void Awake()
        {
            if ((this.array != null) && ((this.fsmTemplateControl.fsmTemplate != null) && Application.isPlaying))
            {
                base.runFsm = base.Fsm.CreateSubFsm(this.fsmTemplateControl);
            }
        }

        protected override void CheckIfFinished()
        {
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

        public override void OnEnter()
        {
            if ((this.array == null) || (base.runFsm == null))
            {
                base.Finish();
            }
            else
            {
                this.currentIndex = 0;
                this.StartFsm();
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

        public override void OnUpdate()
        {
            base.runFsm.Update();
            if (base.runFsm.Finished)
            {
                this.StartNextFsm();
            }
        }

        public override void Reset()
        {
            this.array = null;
            this.fsmTemplateControl = new FsmTemplateControl();
            base.runFsm = null;
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

        private void StartNextFsm()
        {
            this.currentIndex++;
            this.StartFsm();
        }
    }
}

