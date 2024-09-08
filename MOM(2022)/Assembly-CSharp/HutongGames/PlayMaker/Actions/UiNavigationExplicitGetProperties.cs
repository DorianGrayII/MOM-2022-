using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets the explicit navigation properties of a UI Selectable component. ")]
    public class UiNavigationExplicitGetProperties : ComponentAction<Selectable>
    {
        [RequiredField]
        [CheckForComponent(typeof(Selectable))]
        [Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The down Selectable.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject selectOnDown;

        [Tooltip("The up Selectable.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject selectOnUp;

        [Tooltip("The left Selectable.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject selectOnLeft;

        [Tooltip("The right Selectable.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject selectOnRight;

        private Selectable _selectable;

        public override void Reset()
        {
            this.gameObject = null;
            this.selectOnDown = null;
            this.selectOnUp = null;
            this.selectOnLeft = null;
            this.selectOnRight = null;
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

        private void DoGetValue()
        {
            if (this._selectable != null)
            {
                if (!this.selectOnUp.IsNone)
                {
                    this.selectOnUp.Value = ((this._selectable.navigation.selectOnUp == null) ? null : this._selectable.navigation.selectOnUp.gameObject);
                }
                if (!this.selectOnDown.IsNone)
                {
                    this.selectOnDown.Value = ((this._selectable.navigation.selectOnDown == null) ? null : this._selectable.navigation.selectOnDown.gameObject);
                }
                if (!this.selectOnLeft.IsNone)
                {
                    this.selectOnLeft.Value = ((this._selectable.navigation.selectOnLeft == null) ? null : this._selectable.navigation.selectOnLeft.gameObject);
                }
                if (!this.selectOnRight.IsNone)
                {
                    this.selectOnRight.Value = ((this._selectable.navigation.selectOnRight == null) ? null : this._selectable.navigation.selectOnRight.gameObject);
                }
            }
        }
    }
}
