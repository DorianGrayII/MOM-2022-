namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Set the selected value (zero based index) of the UI Dropdown Component")]
    public class UiDropDownSetValue : ComponentAction<Dropdown>
    {
        [RequiredField, CheckForComponent(typeof(Dropdown)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI DropDown component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The selected index of the dropdown (zero based index).")]
        public FsmInt value;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame")]
        public bool everyFrame;
        private Dropdown dropDown;

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.dropDown = base.cachedComponent;
            }
            this.SetValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.SetValue();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.value = null;
            this.everyFrame = false;
        }

        private void SetValue()
        {
            if ((this.dropDown != null) && (this.dropDown.value != this.value.Value))
            {
                this.dropDown.value = this.value.Value;
            }
        }
    }
}

