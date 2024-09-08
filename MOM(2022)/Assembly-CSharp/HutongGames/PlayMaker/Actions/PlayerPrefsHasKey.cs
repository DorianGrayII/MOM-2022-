namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("PlayerPrefs"), HutongGames.PlayMaker.Tooltip("Returns true if key exists in the preferences.")]
    public class PlayerPrefsHasKey : FsmStateAction
    {
        [RequiredField]
        public FsmString key;
        [UIHint(UIHint.Variable), Title("Store Result")]
        public FsmBool variable;
        [HutongGames.PlayMaker.Tooltip("Event to send if key exists.")]
        public FsmEvent trueEvent;
        [HutongGames.PlayMaker.Tooltip("Event to send if key does not exist.")]
        public FsmEvent falseEvent;

        public override void OnEnter()
        {
            base.Finish();
            if (!this.key.IsNone && !this.key.Value.Equals(""))
            {
                this.variable.Value = PlayerPrefs.HasKey(this.key.Value);
            }
            base.Fsm.Event(this.variable.Value ? this.trueEvent : this.falseEvent);
        }

        public override void Reset()
        {
            this.key = "";
        }
    }
}

