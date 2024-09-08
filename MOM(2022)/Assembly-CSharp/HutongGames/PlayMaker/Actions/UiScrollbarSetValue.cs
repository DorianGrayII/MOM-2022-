namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the position value of a UI Scrollbar component. Ranges from 0.0 to 1.0.")]
    public class UiScrollbarSetValue : ComponentAction<Scrollbar>
    {
        [RequiredField, CheckForComponent(typeof(Scrollbar)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Scrollbar component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The position's value of the UI Scrollbar component. Ranges from 0.0 to 1.0."), HasFloatSlider(0f, 1f)]
        public FsmFloat value;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame")]
        public bool everyFrame;
        private Scrollbar scrollbar;
        private float originalValue;

        private void DoSetValue()
        {
            if (this.scrollbar != null)
            {
                this.scrollbar.value = this.value.Value;
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.scrollbar = base.cachedComponent;
            }
            this.originalValue = this.scrollbar.value;
            this.DoSetValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnExit()
        {
            if ((this.scrollbar != null) && this.resetOnExit.Value)
            {
                this.scrollbar.value = this.originalValue;
            }
        }

        public override void OnUpdate()
        {
            this.DoSetValue();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.value = null;
            this.resetOnExit = null;
            this.everyFrame = false;
        }
    }
}

