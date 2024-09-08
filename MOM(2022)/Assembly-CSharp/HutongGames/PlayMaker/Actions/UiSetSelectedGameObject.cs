namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine.EventSystems;

    [ActionCategory(ActionCategory.UI), Tooltip("Sets the EventSystem's currently select GameObject.")]
    public class UiSetSelectedGameObject : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The GameObject to select.")]
        public FsmGameObject gameObject;

        private void DoSetSelectedGameObject()
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject.get_Value());
        }

        public override void OnEnter()
        {
            this.DoSetSelectedGameObject();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
        }
    }
}

