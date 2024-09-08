using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets the isOn value of a UI Toggle component. Optionally send events")]
    public class UiToggleGetIsOn : ComponentAction<Toggle>
    {
        [RequiredField]
        [CheckForComponent(typeof(Toggle))]
        [Tooltip("The GameObject with the UI Toggle component.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("The isOn Value of the UI Toggle component.")]
        public FsmBool value;

        [Tooltip("Event sent when isOn Value is true.")]
        public FsmEvent isOnEvent;

        [Tooltip("Event sent when isOn Value is false.")]
        public FsmEvent isOffEvent;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private Toggle _toggle;

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
                this._toggle = base.cachedComponent;
            }
            this.DoGetValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetValue();
        }

        private void DoGetValue()
        {
            if (!(this._toggle == null))
            {
                this.value.Value = this._toggle.isOn;
                base.Fsm.Event(this._toggle.isOn ? this.isOnEvent : this.isOffEvent);
            }
        }
    }
}
