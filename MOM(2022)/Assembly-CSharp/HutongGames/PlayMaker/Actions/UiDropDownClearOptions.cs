namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Clear the list of options in a UI Dropdown Component")]
    public class UiDropDownClearOptions : ComponentAction<Dropdown>
    {
        [RequiredField, CheckForComponent(typeof(Dropdown)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI DropDown component.")]
        public FsmOwnerDefault gameObject;
        private Dropdown dropDown;

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

        public override void Reset()
        {
            this.gameObject = null;
        }
    }
}

