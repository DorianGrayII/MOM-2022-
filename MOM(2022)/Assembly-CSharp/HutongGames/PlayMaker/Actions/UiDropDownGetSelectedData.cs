namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Get the selected value (zero based index), sprite and text from a UI Dropdown Component")]
    public class UiDropDownGetSelectedData : ComponentAction<Dropdown>
    {
        [RequiredField, CheckForComponent(typeof(Dropdown)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI DropDown component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The selected index of the dropdown (zero based index)."), UIHint(UIHint.Variable)]
        public FsmInt index;
        [HutongGames.PlayMaker.Tooltip("The selected text."), UIHint(UIHint.Variable)]
        public FsmString getText;
        [ObjectType(typeof(Sprite)), HutongGames.PlayMaker.Tooltip("The selected text."), UIHint(UIHint.Variable)]
        public FsmObject getImage;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame")]
        public bool everyFrame;
        private Dropdown dropDown;

        private void GetValue()
        {
            if (this.dropDown != null)
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
                    this.getImage.set_Value(this.dropDown.options[this.dropDown.value].image);
                }
            }
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

        public override void Reset()
        {
            this.gameObject = null;
            this.index = null;
            this.getText = null;
            this.getImage = null;
            this.everyFrame = false;
        }
    }
}

