namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Returns the EventSystem's currently select GameObject.")]
    public class UiGetSelectedGameObject : FsmStateAction
    {
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The currently selected GameObject")]
        public FsmGameObject StoreGameObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Event when the selected GameObject changes")]
        public FsmEvent ObjectChangedEvent;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("If true, each frame will check the currently selected GameObject")]
        public bool everyFrame;
        private GameObject lastGameObject;

        private void GetCurrentSelectedGameObject()
        {
            this.StoreGameObject.set_Value(EventSystem.current.currentSelectedGameObject);
        }

        public override void OnEnter()
        {
            this.GetCurrentSelectedGameObject();
            this.lastGameObject = this.StoreGameObject.get_Value();
        }

        public override void OnUpdate()
        {
            this.GetCurrentSelectedGameObject();
            if ((this.StoreGameObject.get_Value() != this.lastGameObject) && (this.ObjectChangedEvent != null))
            {
                base.Fsm.Event(this.ObjectChangedEvent);
            }
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void Reset()
        {
            this.StoreGameObject = null;
            this.everyFrame = false;
        }
    }
}

