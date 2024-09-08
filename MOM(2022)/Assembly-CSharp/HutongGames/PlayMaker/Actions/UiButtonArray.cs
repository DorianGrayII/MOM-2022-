namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Set up multiple button events in a single action.")]
    public class UiButtonArray : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Where to send the events.")]
        public FsmEventTarget eventTarget;
        [CompoundArray("Buttons", "Button", "Click Event"), CheckForComponent(typeof(Button)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI button component.")]
        public FsmGameObject[] gameObjects;
        [HutongGames.PlayMaker.Tooltip("Send this event when the button is Clicked.")]
        public FsmEvent[] clickEvents;
        [SerializeField]
        private Button[] buttons;
        [SerializeField]
        private GameObject[] cachedGameObjects;
        private UnityAction[] actions;
        private int clickedButton;

        private void InitButtons()
        {
            if ((this.cachedGameObjects == null) || (this.cachedGameObjects.Length != this.gameObjects.Length))
            {
                this.OnPreprocess();
            }
            for (int i = 0; i < this.gameObjects.Length; i++)
            {
                GameObject obj2 = this.gameObjects[i].get_Value();
                if ((obj2 != null) && (this.cachedGameObjects[i] != obj2))
                {
                    this.buttons[i] = obj2.GetComponent<Button>();
                    this.cachedGameObjects[i] = obj2;
                }
            }
        }

        public void OnClick(int index)
        {
            base.Fsm.Event(this.gameObjects[index].get_Value(), this.eventTarget, this.clickEvents[index]);
        }

        public override void OnEnter()
        {
            this.InitButtons();
            for (int i = 0; i < this.buttons.Length; i++)
            {
                Button button = this.buttons[i];
                if (button != null)
                {
                    int index = i;
                    this.actions[i] = () => this.OnClick(index);
                    button.onClick.AddListener(this.actions[i]);
                }
            }
        }

        public override void OnExit()
        {
            for (int i = 0; i < this.gameObjects.Length; i++)
            {
                FsmGameObject obj2 = this.gameObjects[i];
                if (obj2.get_Value() != null)
                {
                    obj2.get_Value().GetComponent<Button>().onClick.RemoveListener(this.actions[i]);
                }
            }
        }

        public override void OnPreprocess()
        {
            this.buttons = new Button[this.gameObjects.Length];
            this.cachedGameObjects = new GameObject[this.gameObjects.Length];
            this.actions = new UnityAction[this.gameObjects.Length];
            this.InitButtons();
        }

        public override void Reset()
        {
            this.gameObjects = new FsmGameObject[3];
            this.clickEvents = new FsmEvent[3];
        }
    }
}

