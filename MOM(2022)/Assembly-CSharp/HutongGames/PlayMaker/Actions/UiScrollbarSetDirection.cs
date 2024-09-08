namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the direction of a UI Scrollbar component.")]
    public class UiScrollbarSetDirection : ComponentAction<Scrollbar>
    {
        [RequiredField, CheckForComponent(typeof(Scrollbar)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Scrollbar component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The direction of the UI Scrollbar."), ObjectType(typeof(Scrollbar.Direction))]
        public FsmEnum direction;
        [HutongGames.PlayMaker.Tooltip("Include the  RectLayouts. Leave to none for no effect")]
        public FsmBool includeRectLayouts;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        private Scrollbar scrollbar;
        private Scrollbar.Direction originalValue;

        private void DoSetValue()
        {
            if (this.scrollbar != null)
            {
                if (this.includeRectLayouts.IsNone)
                {
                    this.scrollbar.direction = (Scrollbar.Direction) this.direction.Value;
                }
                else
                {
                    this.scrollbar.SetDirection((Scrollbar.Direction) this.direction.Value, this.includeRectLayouts.Value);
                }
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.scrollbar = base.cachedComponent;
            }
            if (this.resetOnExit.Value)
            {
                this.originalValue = this.scrollbar.direction;
            }
            this.DoSetValue();
            base.Finish();
        }

        public override void OnExit()
        {
            if ((this.scrollbar != null) && this.resetOnExit.Value)
            {
                if (this.includeRectLayouts.IsNone)
                {
                    this.scrollbar.direction = this.originalValue;
                }
                else
                {
                    this.scrollbar.SetDirection(this.originalValue, this.includeRectLayouts.Value);
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.direction = Scrollbar.Direction.LeftToRight;
            FsmBool bool1 = new FsmBool();
            bool1.UseVariable = true;
            this.includeRectLayouts = bool1;
            this.resetOnExit = null;
        }
    }
}

