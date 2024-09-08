namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    public abstract class BaseUpdateAction : FsmStateAction
    {
        [ActionSection("Update type"), Tooltip("Repeat every frame.")]
        public bool everyFrame;
        public UpdateType updateType;

        protected BaseUpdateAction()
        {
        }

        public abstract void OnActionUpdate();
        public override void OnFixedUpdate()
        {
            if (this.updateType == UpdateType.OnFixedUpdate)
            {
                this.OnActionUpdate();
            }
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnLateUpdate()
        {
            if (this.updateType == UpdateType.OnLateUpdate)
            {
                this.OnActionUpdate();
            }
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnPreprocess()
        {
            if (this.updateType == UpdateType.OnFixedUpdate)
            {
                base.Fsm.HandleFixedUpdate = true;
            }
            else if (this.updateType == UpdateType.OnLateUpdate)
            {
                base.Fsm.HandleLateUpdate = true;
            }
        }

        public override void OnUpdate()
        {
            if (this.updateType == UpdateType.OnUpdate)
            {
                this.OnActionUpdate();
            }
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void Reset()
        {
            this.everyFrame = false;
            this.updateType = UpdateType.OnUpdate;
        }

        public enum UpdateType
        {
            OnUpdate,
            OnLateUpdate,
            OnFixedUpdate
        }
    }
}

