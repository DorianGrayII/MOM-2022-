using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Logic)]
    [Tooltip("Tests if a GameObject has children.")]
    public class GameObjectHasChildren : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject to test.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Event to send if the GameObject has children.")]
        public FsmEvent trueEvent;

        [Tooltip("Event to send if the GameObject does not have children.")]
        public FsmEvent falseEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a bool variable.")]
        public FsmBool storeResult;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = null;
            this.trueEvent = null;
            this.falseEvent = null;
            this.storeResult = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoHasChildren();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoHasChildren();
        }

        private void DoHasChildren()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!(ownerDefaultTarget == null))
            {
                bool flag = ownerDefaultTarget.transform.childCount > 0;
                this.storeResult.Value = flag;
                base.Fsm.Event(flag ? this.trueEvent : this.falseEvent);
            }
        }
    }
}
