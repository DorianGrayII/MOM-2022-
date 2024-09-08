namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Gets the direction of a UI Scrollbar component.")]
    public class UiScrollbarGetDirection : ComponentAction<Scrollbar>
    {
        [RequiredField, CheckForComponent(typeof(Scrollbar)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Scrollbar component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the direction of the UI Scrollbar."), ObjectType(typeof(Scrollbar.Direction))]
        public FsmEnum direction;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame")]
        public bool everyFrame;
        private Scrollbar scrollbar;

        private void DoGetValue()
        {
            if (this.scrollbar != null)
            {
                this.direction.Value = this.scrollbar.direction;
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.scrollbar = base.cachedComponent;
            }
            this.DoGetValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetValue();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.direction = null;
            this.everyFrame = false;
        }
    }
}

