using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Returns the EventSystem's currently select GameObject.")]
    public class UiGetSelectedGameObject : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("The currently selected GameObject")]
        public FsmGameObject StoreGameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Event when the selected GameObject changes")]
        public FsmEvent ObjectChangedEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("If true, each frame will check the currently selected GameObject")]
        public bool everyFrame;

        private GameObject lastGameObject;

        public override void Reset()
        {
            this.StoreGameObject = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.GetCurrentSelectedGameObject();
            this.lastGameObject = this.StoreGameObject.Value;
        }

        public override void OnUpdate()
        {
            this.GetCurrentSelectedGameObject();
            if (this.StoreGameObject.Value != this.lastGameObject && this.ObjectChangedEvent != null)
            {
                base.Fsm.Event(this.ObjectChangedEvent);
            }
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        private void GetCurrentSelectedGameObject()
        {
            this.StoreGameObject.Value = EventSystem.current.currentSelectedGameObject;
        }
    }
}
