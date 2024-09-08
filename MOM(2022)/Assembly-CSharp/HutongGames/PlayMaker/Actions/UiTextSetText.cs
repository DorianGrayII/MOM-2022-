using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the text value of a UI Text component.")]
    public class UiTextSetText : ComponentAction<Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(Text))]
        [Tooltip("The GameObject with the UI Text component.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.TextArea)]
        [Tooltip("The text of the UI Text component.")]
        public FsmString text;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private Text uiText;

        private string originalString;

        public override void Reset()
        {
            this.gameObject = null;
            this.text = null;
            this.resetOnExit = null;
            this.everyFrame = false;
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

        public override void OnUpdate()
        {
            this.DoSetTextValue();
        }

        private void DoSetTextValue()
        {
            if (!(this.uiText == null))
            {
                this.uiText.text = this.text.Value;
            }
        }

        public override void OnExit()
        {
            if (!(this.uiText == null) && this.resetOnExit.Value)
            {
                this.uiText.text = this.originalString;
            }
        }
    }
}
