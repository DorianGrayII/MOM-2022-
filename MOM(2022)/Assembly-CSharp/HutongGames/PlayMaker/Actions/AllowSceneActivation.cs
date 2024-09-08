namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Scene)]
    [Tooltip("Allow scenes to be activated. Use this after LoadSceneAsynch where you did not activated the scene upon loading")]
    public class AllowSceneActivation : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The name of the new scene. It cannot be empty or null, or same as the name of the existing scenes.")]
        public FsmInt aSynchOperationHashCode;

        [Tooltip("Allow the scene to be activated")]
        public FsmBool allowSceneActivation;

        [ActionSection("Result")]
        [Tooltip("The loading's progress.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat progress;

        [Tooltip("True when loading is done")]
        [UIHint(UIHint.Variable)]
        public FsmBool isDone;

        [Tooltip("Event sent when scene loading is done")]
        public FsmEvent doneEvent;

        [Tooltip("Event sent when action could not be performed. Check Log for more information")]
        public FsmEvent failureEvent;

        public override void Reset()
        {
            this.aSynchOperationHashCode = null;
            this.allowSceneActivation = true;
            this.progress = null;
            this.isDone = null;
            this.doneEvent = null;
            this.failureEvent = null;
        }

        public override void OnEnter()
        {
            this.DoAllowSceneActivation();
        }

        public override void OnUpdate()
        {
            if (!this.progress.IsNone)
            {
                this.progress.Value = LoadSceneAsynch.aSyncOperationLUT[this.aSynchOperationHashCode.Value].progress;
            }
            if (!this.isDone.IsNone)
            {
                this.isDone.Value = LoadSceneAsynch.aSyncOperationLUT[this.aSynchOperationHashCode.Value].isDone;
            }
            if (LoadSceneAsynch.aSyncOperationLUT[this.aSynchOperationHashCode.Value].isDone)
            {
                LoadSceneAsynch.aSyncOperationLUT.Remove(this.aSynchOperationHashCode.Value);
                base.Fsm.Event(this.doneEvent);
                base.Finish();
            }
        }

        private void DoAllowSceneActivation()
        {
            if (this.aSynchOperationHashCode.IsNone || this.allowSceneActivation.IsNone || LoadSceneAsynch.aSyncOperationLUT == null || !LoadSceneAsynch.aSyncOperationLUT.ContainsKey(this.aSynchOperationHashCode.Value))
            {
                base.Fsm.Event(this.failureEvent);
                base.Finish();
            }
            else
            {
                LoadSceneAsynch.aSyncOperationLUT[this.aSynchOperationHashCode.Value].allowSceneActivation = this.allowSceneActivation.Value;
            }
        }
    }
}
