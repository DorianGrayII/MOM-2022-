using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("PlayerPrefs")]
    [Tooltip("Sets the value of the preference identified by key.")]
    public class PlayerPrefsSetFloat : FsmStateAction
    {
        [CompoundArray("Count", "Key", "Value")]
        [Tooltip("Case sensitive key.")]
        public FsmString[] keys;

        public FsmFloat[] values;

        public override void Reset()
        {
            this.keys = new FsmString[1];
            this.values = new FsmFloat[1];
        }

        public override void OnEnter()
        {
            for (int i = 0; i < this.keys.Length; i++)
            {
                if (!this.keys[i].IsNone || !this.keys[i].Value.Equals(""))
                {
                    PlayerPrefs.SetFloat(this.keys[i].Value, this.values[i].IsNone ? 0f : this.values[i].Value);
                }
            }
            base.Finish();
        }
    }
}
