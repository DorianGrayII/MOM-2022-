using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets the direction of a UI Scrollbar component.")]
    public class UiScrollbarGetDirection : ComponentAction<Scrollbar>
    {
        [RequiredField]
        [CheckForComponent(typeof(Scrollbar))]
        [Tooltip("The GameObject with the UI Scrollbar component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the direction of the UI Scrollbar.")]
        [ObjectType(typeof(Scrollbar.Direction))]
        public FsmEnum direction;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private Scrollbar scrollbar;

        public override void Reset()
        {
            this.gameObject = null;
            this.direction = null;
            this.everyFrame = false;
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

        private void DoGetValue()
        {
            if (this.scrollbar != null)
            {
                this.direction.Value = this.scrollbar.direction;
            }
        }
    }
}
