namespace HutongGames.PlayMaker
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class PlayMakerUiEventBase : MonoBehaviour
    {
        public List<PlayMakerFSM> targetFsms = new List<PlayMakerFSM>();
        [NonSerialized]
        protected bool initialized;

        protected PlayMakerUiEventBase()
        {
        }

        public void AddTargetFsm(PlayMakerFSM fsm)
        {
            if (!this.TargetsFsm(fsm))
            {
                this.targetFsms.Add(fsm);
            }
            this.Initialize();
        }

        protected virtual void Initialize()
        {
            this.initialized = true;
        }

        protected void OnEnable()
        {
            this.Initialize();
        }

        public void PreProcess()
        {
            this.Initialize();
        }

        protected void SendEvent(FsmEvent fsmEvent)
        {
            for (int i = 0; i < this.targetFsms.Count; i++)
            {
                this.targetFsms[i].Fsm.Event(base.gameObject, fsmEvent);
            }
        }

        private bool TargetsFsm(PlayMakerFSM fsm)
        {
            for (int i = 0; i < this.targetFsms.Count; i++)
            {
                PlayMakerFSM rfsm = this.targetFsms[i];
                if (fsm == rfsm)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

