using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Get the selected value (zero based index), sprite and text from a UI Dropdown Component")]
    public class UiDropDownGetSelectedData : ComponentAction<Dropdown>
    {
        [RequiredField]
        [CheckForComponent(typeof(Dropdown))]
        [Tooltip("The GameObject with the UI DropDown component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The selected index of the dropdown (zero based index).")]
        [UIHint(UIHint.Variable)]
        public FsmInt index;

        [Tooltip("The selected text.")]
        [UIHint(UIHint.Variable)]
        public FsmString getText;

        [ObjectType(typeof(Sprite))]
        [Tooltip("The selected text.")]
        [UIHint(UIHint.Variable)]
        public FsmObject getImage;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private Dropdown dropDown;

        public override void Reset()
        {
            this.gameObject = null;
            this.index = null;
            this.getText = null;
            this.getImage = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.dropDown = base.cachedComponent;
            }
            this.GetValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.GetValue();
        }

        private void GetValue()
        {
            if (!(this.dropDown == null))
            {
                if (!this.index.IsNone)
                {
                    this.index.Value = this.dropDown.value;
                }
                if (!this.getText.IsNone)
                {
                    this.getText.Value = this.dropDown.options[this.dropDown.value].text;
                }
                if (!this.getImage.IsNone)
                {
                    this.getImage.Value = this.dropDown.options[this.dropDown.value].image;
                }
            }
        }
    }
}
