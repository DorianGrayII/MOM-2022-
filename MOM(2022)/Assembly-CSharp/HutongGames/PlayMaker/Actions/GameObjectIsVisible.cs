namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Logic), ActionTarget(typeof(GameObject), "gameObject", false), HutongGames.PlayMaker.Tooltip("Tests if a Game Object is visible.")]
    public class GameObjectIsVisible : ComponentAction<Renderer>
    {
        [RequiredField, CheckForComponent(typeof(Renderer)), HutongGames.PlayMaker.Tooltip("The GameObject to test.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Event to send if the GameObject is visible.")]
        public FsmEvent trueEvent;
        [HutongGames.PlayMaker.Tooltip("Event to send if the GameObject is NOT visible.")]
        public FsmEvent falseEvent;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the result in a bool variable.")]
        public FsmBool storeResult;
        public bool everyFrame;

        private void DoIsVisible()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                bool isVisible = base.renderer.isVisible;
                this.storeResult.Value = isVisible;
                base.Fsm.Event(isVisible ? this.trueEvent : this.falseEvent);
            }
        }

        public override void OnEnter()
        {
            this.DoIsVisible();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoIsVisible();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.trueEvent = null;
            this.falseEvent = null;
            this.storeResult = null;
            this.everyFrame = false;
        }
    }
}

