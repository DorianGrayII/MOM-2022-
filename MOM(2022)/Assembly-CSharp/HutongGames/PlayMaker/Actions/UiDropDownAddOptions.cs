using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Add multiple options to the options of the Dropdown UI Component")]
    public class UiDropDownAddOptions : ComponentAction<Dropdown>
    {
        [RequiredField]
        [CheckForComponent(typeof(Dropdown))]
        [Tooltip("The GameObject with the UI DropDown component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The Options.")]
        [CompoundArray("Options", "Text", "Image")]
        public FsmString[] optionText;

        [ObjectType(typeof(Sprite))]
        public FsmObject[] optionImage;

        private Dropdown dropDown;

        private List<Dropdown.OptionData> options;

        public override void Reset()
        {
            this.gameObject = null;
            this.optionText = new FsmString[1];
            this.optionImage = new FsmObject[1];
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

        private void DoAddOptions()
        {
            if (!(this.dropDown == null))
            {
                this.options = new List<Dropdown.OptionData>();
                for (int i = 0; i < this.optionText.Length; i++)
                {
                    FsmString fsmString = this.optionText[i];
                    this.options.Add(new Dropdown.OptionData
                    {
                        text = fsmString.Value,
                        image = (this.optionImage[i].RawValue as Sprite)
                    });
                }
                this.dropDown.AddOptions(this.options);
            }
        }
    }
}
