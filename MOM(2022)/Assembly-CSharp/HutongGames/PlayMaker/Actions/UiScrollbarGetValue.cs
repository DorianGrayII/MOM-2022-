using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets the value of a UI Scrollbar component.")]
    public class UiScrollbarGetValue : ComponentAction<Scrollbar>
    {
        [RequiredField]
        [CheckForComponent(typeof(Scrollbar))]
        [Tooltip("The GameObject with the UI Scrollbar component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The position value of the UI Scrollbar.")]
        public FsmFloat value;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private Scrollbar scrollbar;

        public override void Reset()
        {
            this.gameObject = null;
            this.value = null;
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
                this.value.Value = this.scrollbar.value;
            }
        }
    }
}
