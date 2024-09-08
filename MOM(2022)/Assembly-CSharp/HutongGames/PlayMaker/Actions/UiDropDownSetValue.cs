using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Set the selected value (zero based index) of the UI Dropdown Component")]
    public class UiDropDownSetValue : ComponentAction<Dropdown>
    {
        [RequiredField]
        [CheckForComponent(typeof(Dropdown))]
        [Tooltip("The GameObject with the UI DropDown component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The selected index of the dropdown (zero based index).")]
        public FsmInt value;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private Dropdown dropDown;

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

        private void SetValue()
        {
            if (!(this.dropDown == null) && this.dropDown.value != this.value.Value)
            {
                this.dropDown.value = this.value.Value;
            }
        }
    }
}
