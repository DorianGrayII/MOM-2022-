using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GUILayout)]
    [Tooltip("GUILayout Toolbar. NOTE: Arrays must be the same length as NumButtons or empty.")]
    public class GUILayoutToolbar : GUILayoutAction
    {
        [Tooltip("The number of buttons in the toolbar")]
        public FsmInt numButtons;

        [Tooltip("Store the index of the selected button in an Integer Variable")]
        [UIHint(UIHint.Variable)]
        public FsmInt selectedButton;

        [Tooltip("Event to send when each button is pressed.")]
        public FsmEvent[] buttonEventsArray;

        [Tooltip("Image to use on each button.")]
        public FsmTexture[] imagesArray;

        [Tooltip("Text to use on each button.")]
        public FsmString[] textsArray;

        [Tooltip("Tooltip to use for each button.")]
        public FsmString[] tooltipsArray;

        [Tooltip("A named GUIStyle to use for the toolbar buttons. Default is Button.")]
        public FsmString style;

        [Tooltip("Update the content of the buttons every frame. Useful if the buttons are using variables that change.")]
        public bool everyFrame;

        private GUIContent[] contents;

        public GUIContent[] Contents
        {
            get
            {
                if (this.contents == null)
                {
                    this.SetButtonsContent();
                }
                return this.contents;
            }
        }

        private void SetButtonsContent()
        {
            if (this.contents == null)
            {
                this.contents = new GUIContent[this.numButtons.Value];
            }
            for (int i = 0; i < this.numButtons.Value; i++)
            {
                this.contents[i] = new GUIContent();
            }
            for (int j = 0; j < this.imagesArray.Length; j++)
            {
                this.contents[j].image = this.imagesArray[j].Value;
            }
            for (int k = 0; k < this.textsArray.Length; k++)
            {
                this.contents[k].text = this.textsArray[k].Value;
            }
            for (int l = 0; l < this.tooltipsArray.Length; l++)
            {
                this.contents[l].tooltip = this.tooltipsArray[l].Value;
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.numButtons = 0;
            this.selectedButton = null;
            this.buttonEventsArray = new FsmEvent[0];
            this.imagesArray = new FsmTexture[0];
            this.tooltipsArray = new FsmString[0];
            this.style = "Button";
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            string text = this.ErrorCheck();
            if (!string.IsNullOrEmpty(text))
            {
                base.LogError(text);
                base.Finish();
            }
        }

        public override void OnGUI()
        {
            if (this.everyFrame)
            {
                this.SetButtonsContent();
            }
            bool changed = GUI.changed;
            GUI.changed = false;
            this.selectedButton.Value = GUILayout.Toolbar(this.selectedButton.Value, this.Contents, this.style.Value, base.LayoutOptions);
            if (GUI.changed)
            {
                if (this.selectedButton.Value < this.buttonEventsArray.Length)
                {
                    base.Fsm.Event(this.buttonEventsArray[this.selectedButton.Value]);
                    GUIUtility.ExitGUI();
                }
            }
            else
            {
                GUI.changed = changed;
            }
        }

        public override string ErrorCheck()
        {
            string text = "";
            if (this.imagesArray.Length != 0 && this.imagesArray.Length != this.numButtons.Value)
            {
                text += "Images array doesn't match NumButtons.\n";
            }
            if (this.textsArray.Length != 0 && this.textsArray.Length != this.numButtons.Value)
            {
                text += "Texts array doesn't match NumButtons.\n";
            }
            if (this.tooltipsArray.Length != 0 && this.tooltipsArray.Length != this.numButtons.Value)
            {
                text += "Tooltips array doesn't match NumButtons.\n";
            }
            return text;
        }
    }
}
