using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Logic)]
    [Tooltip("Sends an Event based on a Game Object's Tag.")]
    public class GameObjectTagSwitch : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The GameObject to test.")]
        public FsmGameObject gameObject;

        [CompoundArray("Tag Switches", "Compare Tag", "Send Event")]
        [UIHint(UIHint.Tag)]
        public FsmString[] compareTo;

        public FsmEvent[] sendEvent;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = null;
            this.compareTo = new FsmString[1];
            this.sendEvent = new FsmEvent[1];
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoTagSwitch();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoTagSwitch();
        }

        private void DoTagSwitch()
        {
            GameObject value = this.gameObject.Value;
            if (value == null)
            {
                return;
            }
            for (int i = 0; i < this.compareTo.Length; i++)
            {
                if (value.tag == this.compareTo[i].Value)
                {
                    base.Fsm.Event(this.sendEvent[i]);
                    break;
                }
            }
        }
    }
}
