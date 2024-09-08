namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Logic), HutongGames.PlayMaker.Tooltip("Sends an Event based on a Game Object's Tag.")]
    public class GameObjectTagSwitch : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The GameObject to test.")]
        public FsmGameObject gameObject;
        [CompoundArray("Tag Switches", "Compare Tag", "Send Event"), UIHint(UIHint.Tag)]
        public FsmString[] compareTo;
        public FsmEvent[] sendEvent;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoTagSwitch()
        {
            GameObject obj2 = this.gameObject.get_Value();
            if (obj2 != null)
            {
                for (int i = 0; i < this.compareTo.Length; i++)
                {
                    if (obj2.tag == this.compareTo[i].Value)
                    {
                        base.Fsm.Event(this.sendEvent[i]);
                        return;
                    }
                }
            }
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

        public override void Reset()
        {
            this.gameObject = null;
            this.compareTo = new FsmString[1];
            this.sendEvent = new FsmEvent[1];
            this.everyFrame = false;
        }
    }
}

