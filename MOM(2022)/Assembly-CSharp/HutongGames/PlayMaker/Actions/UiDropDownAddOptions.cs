namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Add multiple options to the options of the Dropdown UI Component")]
    public class UiDropDownAddOptions : ComponentAction<Dropdown>
    {
        [RequiredField, CheckForComponent(typeof(Dropdown)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI DropDown component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The Options."), CompoundArray("Options", "Text", "Image")]
        public FsmString[] optionText;
        [ObjectType(typeof(Sprite))]
        public FsmObject[] optionImage;
        private Dropdown dropDown;
        private List<Dropdown.OptionData> options;

        private void DoAddOptions()
        {
            if (this.dropDown != null)
            {
                this.options = new List<Dropdown.OptionData>();
                for (int i = 0; i < this.optionText.Length; i++)
                {
                    FsmString str = this.optionText[i];
                    Dropdown.OptionData item = new Dropdown.OptionData();
                    item.text = str.Value;
                    item.image = this.optionImage[i].RawValue as Sprite;
                    this.options.Add(item);
                }
                this.dropDown.AddOptions(this.options);
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.dropDown = base.cachedComponent;
            }
            this.DoAddOptions();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.optionText = new FsmString[1];
            this.optionImage = new FsmObject[1];
        }
    }
}

