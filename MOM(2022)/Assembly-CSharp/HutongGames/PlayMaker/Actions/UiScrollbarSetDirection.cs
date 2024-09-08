using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the direction of a UI Scrollbar component.")]
    public class UiScrollbarSetDirection : ComponentAction<Scrollbar>
    {
        [RequiredField]
        [CheckForComponent(typeof(Scrollbar))]
        [Tooltip("The GameObject with the UI Scrollbar component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The direction of the UI Scrollbar.")]
        [ObjectType(typeof(Scrollbar.Direction))]
        public FsmEnum direction;

        [Tooltip("Include the  RectLayouts. Leave to none for no effect")]
        public FsmBool includeRectLayouts;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        private Scrollbar scrollbar;

        private Scrollbar.Direction originalValue;

        public override void Reset()
        {
            this.gameObject = null;
            this.direction = Scrollbar.Direction.LeftToRight;
            this.includeRectLayouts = new FsmBool
            {
                UseVariable = true
            };
            this.resetOnExit = null;
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

        private void DoSetValue()
        {
            if (!(this.scrollbar == null))
            {
                if (this.includeRectLayouts.IsNone)
                {
                    this.scrollbar.direction = (Scrollbar.Direction)(object)this.direction.Value;
                }
                else
                {
                    this.scrollbar.SetDirection((Scrollbar.Direction)(object)this.direction.Value, this.includeRectLayouts.Value);
                }
            }
        }

        public override void OnExit()
        {
            if (!(this.scrollbar == null) && this.resetOnExit.Value)
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
    }
}
