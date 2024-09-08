using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Set up multiple button events in a single action.")]
    public class UiButtonArray : FsmStateAction
    {
        [Tooltip("Where to send the events.")]
        public FsmEventTarget eventTarget;

        [CompoundArray("Buttons", "Button", "Click Event")]
        [CheckForComponent(typeof(Button))]
        [Tooltip("The GameObject with the UI button component.")]
        public FsmGameObject[] gameObjects;

        [Tooltip("Send this event when the button is Clicked.")]
        public FsmEvent[] clickEvents;

        [SerializeField]
        private Button[] buttons;

        [SerializeField]
        private GameObject[] cachedGameObjects;

        private UnityAction[] actions;

        private int clickedButton;

        public override void Reset()
        {
            this.gameObjects = new FsmGameObject[3];
            this.clickEvents = new FsmEvent[3];
        }

        public override void OnPreprocess()
        {
            this.buttons = new Button[this.gameObjects.Length];
            this.cachedGameObjects = new GameObject[this.gameObjects.Length];
            this.actions = new UnityAction[this.gameObjects.Length];
            this.InitButtons();
        }

        private void InitButtons()
        {
            if (this.cachedGameObjects == null || this.cachedGameObjects.Length != this.gameObjects.Length)
            {
                this.OnPreprocess();
            }
            for (int i = 0; i < this.gameObjects.Length; i++)
            {
                GameObject value = this.gameObjects[i].Value;
                if (value != null && this.cachedGameObjects[i] != value)
                {
                    this.buttons[i] = value.GetComponent<Button>();
                    this.cachedGameObjects[i] = value;
                }
            }
        }

        public override void OnEnter()
        {
            this.InitButtons();
            for (int i = 0; i < this.buttons.Length; i++)
            {
                Button button = this.buttons[i];
                if (!(button == null))
                {
                    int index = i;
                    this.actions[i] = delegate
                    {
                        this.OnClick(index);
                    };
                    button.onClick.AddListener(this.actions[i]);
                }
            }
        }

        public override void OnExit()
        {
            for (int i = 0; i < this.gameObjects.Length; i++)
            {
                FsmGameObject fsmGameObject = this.gameObjects[i];
                if (!(fsmGameObject.Value == null))
                {
                    fsmGameObject.Value.GetComponent<Button>().onClick.RemoveListener(this.actions[i]);
                }
            }
        }

        public void OnClick(int index)
        {
            base.Fsm.Event(this.gameObjects[index].Value, this.eventTarget, this.clickEvents[index]);
        }
    }
}
