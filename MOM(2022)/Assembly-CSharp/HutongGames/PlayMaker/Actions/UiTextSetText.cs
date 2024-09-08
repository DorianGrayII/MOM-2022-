namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the text value of a UI Text component.")]
    public class UiTextSetText : ComponentAction<Text>
    {
        [RequiredField, CheckForComponent(typeof(Text)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Text component.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.TextArea), HutongGames.PlayMaker.Tooltip("The text of the UI Text component.")]
        public FsmString text;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame")]
        public bool everyFrame;
        private Text uiText;
        private string originalString;

        private void DoSetTextValue()
        {
            if (this.uiText != null)
            {
                this.uiText.text = this.text.Value;
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.uiText = base.cachedComponent;
            }
            this.originalString = this.uiText.text;
            this.DoSetTextValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnExit()
        {
            if ((this.uiText != null) && this.resetOnExit.Value)
            {
                this.uiText.text = this.originalString;
            }
        }

        public override void OnUpdate()
        {
            this.DoSetTextValue();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.text = null;
            this.resetOnExit = null;
            this.everyFrame = false;
        }
    }
}

