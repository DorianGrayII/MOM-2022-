using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker
{
    public abstract class PlayMakerUiEventBase : MonoBehaviour
    {
        public List<PlayMakerFSM> targetFsms = new List<PlayMakerFSM>();

        [NonSerialized]
        protected bool initialized;

        public void AddTargetFsm(PlayMakerFSM fsm)
        {
            if (!this.TargetsFsm(fsm))
            {
                this.targetFsms.Add(fsm);
            }
            this.Initialize();
        }

        private bool TargetsFsm(PlayMakerFSM fsm)
        {
            for (int i = 0; i < this.targetFsms.Count; i++)
            {
                PlayMakerFSM playMakerFSM = this.targetFsms[i];
                if (fsm == playMakerFSM)
                {
                    return true;
                }
            }
            return false;
        }

        protected void OnEnable()
        {
            this.Initialize();
        }

        public void PreProcess()
        {
            this.Initialize();
        }

        protected virtual void Initialize()
        {
            this.initialized = true;
        }

        protected void SendEvent(FsmEvent fsmEvent)
        {
            for (int i = 0; i < this.targetFsms.Count; i++)
            {
                this.targetFsms[i].Fsm.Event(base.gameObject, fsmEvent);
            }
        }
    }
}
