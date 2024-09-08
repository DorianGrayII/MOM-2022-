using HutongGames.PlayMaker;
using MHUtils;

namespace MOM
{
    public abstract class FSMStateBase : FsmStateAction
    {
        private MHTimer t;

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
            MHEventSystem.UnRegisterListenersLinkedToObject(this);
        }

        public virtual string GetStageName()
        {
            return base.GetType().Name;
        }
    }
}
