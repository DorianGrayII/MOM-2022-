using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("PlayerPrefs")]
    [Tooltip("Sets the value of the preference identified by key.")]
    public class PlayerPrefsSetString : FsmStateAction
    {
        [CompoundArray("Count", "Key", "Value")]
        [Tooltip("Case sensitive key.")]
        public FsmString[] keys;

        public FsmString[] values;

        public override void Reset()
        {
            this.keys = new FsmString[1];
            this.values = new FsmString[1];
        }

        public override void OnEnter()
        {
            for (int i = 0; i < this.keys.Length; i++)
            {
                if (!this.keys[i].IsNone || !this.keys[i].Value.Equals(""))
                {
                    PlayerPrefs.SetString(this.keys[i].Value, this.values[i].IsNone ? "" : this.values[i].Value);
                }
            }
            base.Finish();
        }
    }
}
