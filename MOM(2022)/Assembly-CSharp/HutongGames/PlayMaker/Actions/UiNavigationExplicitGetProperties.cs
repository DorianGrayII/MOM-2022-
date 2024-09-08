namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Gets the explicit navigation properties of a UI Selectable component. ")]
    public class UiNavigationExplicitGetProperties : ComponentAction<Selectable>
    {
        [RequiredField, CheckForComponent(typeof(Selectable)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The down Selectable."), UIHint(UIHint.Variable)]
        public FsmGameObject selectOnDown;
        [HutongGames.PlayMaker.Tooltip("The up Selectable."), UIHint(UIHint.Variable)]
        public FsmGameObject selectOnUp;
        [HutongGames.PlayMaker.Tooltip("The left Selectable."), UIHint(UIHint.Variable)]
        public FsmGameObject selectOnLeft;
        [HutongGames.PlayMaker.Tooltip("The right Selectable."), UIHint(UIHint.Variable)]
        public FsmGameObject selectOnRight;
        private Selectable _selectable;

        private void DoGetValue()
        {
            if (this._selectable != null)
            {
                if (!this.selectOnUp.IsNone)
                {
                    this.selectOnUp.set_Value(this._selectable.navigation.selectOnUp?.gameObject);
                }
                if (!this.selectOnDown.IsNone)
                {
                    this.selectOnDown.set_Value(this._selectable.navigation.selectOnDown?.gameObject);
                }
                if (!this.selectOnLeft.IsNone)
                {
                    this.selectOnLeft.set_Value(this._selectable.navigation.selectOnLeft?.gameObject);
                }
                if (!this.selectOnRight.IsNone)
                {
                    this.selectOnRight.set_Value(this._selectable.navigation.selectOnRight?.gameObject);
                }
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this._selectable = ownerDefaultTarget.GetComponent<Selectable>();
            }
            this.DoGetValue();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.selectOnDown = null;
            this.selectOnUp = null;
            this.selectOnLeft = null;
            this.selectOnRight = null;
        }
    }
}

