using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets the text value of a UI Text component.")]
    public class UiTextGetText : ComponentAction<Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(Text))]
        [Tooltip("The GameObject with the UI Text component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The text value of the UI Text component.")]
        public FsmString text;

        [Tooltip("Runs every frame. Useful to animate values over time.")]
        public bool everyFrame;

        private Text uiText;

        public override void Reset()
        {
            this.text = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.uiText = base.cachedComponent;
            }
            this.DoGetTextValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetTextValue();
        }

        private void DoGetTextValue()
        {
            if (this.uiText != null)
            {
                this.text.Value = this.uiText.text;
            }
        }
    }
}
