using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Clear the list of options in a UI Dropdown Component")]
    public class UiDropDownClearOptions : ComponentAction<Dropdown>
    {
        [RequiredField]
        [CheckForComponent(typeof(Dropdown))]
        [Tooltip("The GameObject with the UI DropDown component.")]
        public FsmOwnerDefault gameObject;

        private Dropdown dropDown;

        public override void Reset()
        {
            this.gameObject = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.dropDown = base.cachedComponent;
            }
            if (this.dropDown != null)
            {
                this.dropDown.ClearOptions();
            }
            base.Finish();
        }
    }
}
