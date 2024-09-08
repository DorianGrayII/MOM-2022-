namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the isOn property of a UI Toggle component.")]
    public class UiToggleSetIsOn : ComponentAction<Toggle>
    {
        [RequiredField, CheckForComponent(typeof(Toggle)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Toggle component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Should the toggle be on?")]
        public FsmBool isOn;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        private Toggle _toggle;
        private bool _originalValue;

        private void DoSetValue()
        {
            if (this._toggle != null)
            {
                this._originalValue = this._toggle.isOn;
                this._toggle.isOn = this.isOn.Value;
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this._toggle = base.cachedComponent;
            }
            this.DoSetValue();
            base.Finish();
        }

        public override void OnExit()
        {
            if ((this._toggle != null) && this.resetOnExit.Value)
            {
                this._toggle.isOn = this._originalValue;
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.isOn = null;
            this.resetOnExit = null;
        }
    }
}

