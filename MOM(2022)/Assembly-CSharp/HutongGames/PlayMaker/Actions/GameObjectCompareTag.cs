namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Logic), Tooltip("Tests if a Game Object has a tag.")]
    public class GameObjectCompareTag : FsmStateAction
    {
        [RequiredField, Tooltip("The GameObject to test.")]
        public FsmGameObject gameObject;
        [RequiredField, UIHint(UIHint.Tag), Tooltip("The Tag to check for.")]
        public FsmString tag;
        [Tooltip("Event to send if the GameObject has the Tag.")]
        public FsmEvent trueEvent;
        [Tooltip("Event to send if the GameObject does not have the Tag.")]
        public FsmEvent falseEvent;
        [UIHint(UIHint.Variable), Tooltip("Store the result in a Bool variable.")]
        public FsmBool storeResult;
        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoCompareTag()
        {
            bool flag = false;
            if (this.gameObject.get_Value() != null)
            {
                flag = this.gameObject.get_Value().CompareTag(this.tag.Value);
            }
            this.storeResult.Value = flag;
            base.Fsm.Event(flag ? this.trueEvent : this.falseEvent);
        }

        public override void OnEnter()
        {
            this.DoCompareTag();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoCompareTag();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.tag = "Untagged";
            this.trueEvent = null;
            this.falseEvent = null;
            this.storeResult = null;
            this.everyFrame = false;
        }
    }
}

