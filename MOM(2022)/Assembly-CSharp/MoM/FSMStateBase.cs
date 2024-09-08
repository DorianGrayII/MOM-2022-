namespace MOM
{
    using HutongGames.PlayMaker;
    using MHUtils;
    using System;

    public abstract class FSMStateBase : FsmStateAction
    {
        private MHTimer t;

        protected FSMStateBase()
        {
        }

        public virtual string GetStageName()
        {
            return base.GetType().Name;
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
            MHEventSystem.UnRegisterListenersLinkedToObject(this);
        }
    }
}

