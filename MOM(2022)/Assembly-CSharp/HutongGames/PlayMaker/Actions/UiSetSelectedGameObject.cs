using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the EventSystem's currently select GameObject.")]
    public class UiSetSelectedGameObject : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The GameObject to select.")]
        public FsmGameObject gameObject;

        public override void Reset()
        {
            this.gameObject = null;
        }

        public override void OnEnter()
        {
            this.DoSetSelectedGameObject();
            base.Finish();
        }

        private void DoSetSelectedGameObject()
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject.Value);
        }
    }
}
