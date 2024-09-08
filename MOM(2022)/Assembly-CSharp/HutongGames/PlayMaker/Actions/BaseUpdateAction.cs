namespace HutongGames.PlayMaker.Actions
{
    public abstract class BaseUpdateAction : FsmStateAction
    {
        public enum UpdateType
        {
            OnUpdate = 0,
            OnLateUpdate = 1,
            OnFixedUpdate = 2
        }

        [ActionSection("Update type")]
        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public UpdateType updateType;

        public abstract void OnActionUpdate();

        public override void Reset()
        {
            this.everyFrame = false;
            this.updateType = UpdateType.OnUpdate;
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
    }
}
