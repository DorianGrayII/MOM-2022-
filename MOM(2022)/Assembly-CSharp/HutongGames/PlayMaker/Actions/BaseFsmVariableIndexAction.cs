using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
    public abstract class BaseFsmVariableIndexAction : FsmStateAction
    {
        [ActionSection("Events")]
        [Tooltip("The event to trigger if the index is out of range")]
        public FsmEvent indexOutOfRange;

        [Tooltip("The event to send if the FSM is not found.")]
        public FsmEvent fsmNotFound;

        [Tooltip("The event to send if the Variable is not found.")]
        public FsmEvent variableNotFound;

        private GameObject cachedGameObject;

        private string cachedFsmName;

        protected PlayMakerFSM fsm;

        public override void Reset()
        {
            this.fsmNotFound = null;
            this.variableNotFound = null;
        }

        protected bool UpdateCache(GameObject go, string fsmName)
        {
            if (go == null)
            {
                return false;
            }
            if (this.fsm == null || this.cachedGameObject != go || this.cachedFsmName != fsmName)
            {
                this.fsm = ActionHelpers.GetGameObjectFsm(go, fsmName);
                this.cachedGameObject = go;
                this.cachedFsmName = fsmName;
                if (this.fsm == null)
                {
                    base.LogWarning("Could not find FSM: " + fsmName);
                    base.Fsm.Event(this.fsmNotFound);
                }
            }
            return true;
        }

        protected void DoVariableNotFound(string variableName)
        {
            base.LogWarning("Could not find variable: " + variableName);
            base.Fsm.Event(this.variableNotFound);
        }
    }
}
