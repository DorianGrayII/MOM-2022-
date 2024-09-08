namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the explicit navigation properties of a UI Selectable component. Note that it will have no effect until Navigation mode is set to 'Explicit'.")]
    public class UiNavigationExplicitSetProperties : ComponentAction<Selectable>
    {
        [RequiredField, CheckForComponent(typeof(Selectable)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The down Selectable. Leave as None for no effect"), CheckForComponent(typeof(Selectable))]
        public FsmGameObject selectOnDown;
        [HutongGames.PlayMaker.Tooltip("The up Selectable.  Leave as None for no effect"), CheckForComponent(typeof(Selectable))]
        public FsmGameObject selectOnUp;
        [HutongGames.PlayMaker.Tooltip("The left Selectable.  Leave as None for no effect"), CheckForComponent(typeof(Selectable))]
        public FsmGameObject selectOnLeft;
        [HutongGames.PlayMaker.Tooltip("The right Selectable.  Leave as None for no effect"), CheckForComponent(typeof(Selectable))]
        public FsmGameObject selectOnRight;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        private Selectable selectable;
        private Navigation navigation;
        private Navigation originalState;

        private void DoSetValue()
        {
            if (this.selectable != null)
            {
                this.navigation = this.selectable.navigation;
                if (!this.selectOnDown.IsNone)
                {
                    this.navigation.selectOnDown = GetComponentFromFsmGameObject<Selectable>(this.selectOnDown);
                }
                if (!this.selectOnUp.IsNone)
                {
                    this.navigation.selectOnUp = GetComponentFromFsmGameObject<Selectable>(this.selectOnUp);
                }
                if (!this.selectOnLeft.IsNone)
                {
                    this.navigation.selectOnLeft = GetComponentFromFsmGameObject<Selectable>(this.selectOnLeft);
                }
                if (!this.selectOnRight.IsNone)
                {
                    this.navigation.selectOnRight = GetComponentFromFsmGameObject<Selectable>(this.selectOnRight);
                }
                this.selectable.navigation = this.navigation;
            }
        }

        private static T GetComponentFromFsmGameObject<T>(FsmGameObject variable) where T: Component
        {
            if (variable.get_Value() != null)
            {
                return variable.get_Value().GetComponent<T>();
            }
            return default(T);
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.selectable = base.cachedComponent;
            }
            if ((this.selectable != null) && this.resetOnExit.Value)
            {
                this.originalState = this.selectable.navigation;
            }
            this.DoSetValue();
            base.Finish();
        }

        public override void OnExit()
        {
            if ((this.selectable != null) && this.resetOnExit.Value)
            {
                this.navigation = this.selectable.navigation;
                this.navigation.selectOnDown = this.originalState.selectOnDown;
                this.navigation.selectOnLeft = this.originalState.selectOnLeft;
                this.navigation.selectOnRight = this.originalState.selectOnRight;
                this.navigation.selectOnUp = this.originalState.selectOnUp;
                this.selectable.navigation = this.navigation;
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            FsmGameObject obj1 = new FsmGameObject();
            obj1.UseVariable = true;
            this.selectOnDown = obj1;
            FsmGameObject obj2 = new FsmGameObject();
            obj2.UseVariable = true;
            this.selectOnUp = obj2;
            FsmGameObject obj3 = new FsmGameObject();
            obj3.UseVariable = true;
            this.selectOnLeft = obj3;
            FsmGameObject obj4 = new FsmGameObject();
            obj4.UseVariable = true;
            this.selectOnRight = obj4;
            this.resetOnExit = false;
        }
    }
}

