using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("PlayerPrefs")]
    [Tooltip("Returns the value corresponding to key in the preference file if it exists.")]
    public class PlayerPrefsGetFloat : FsmStateAction
    {
        [CompoundArray("Count", "Key", "Variable")]
        [Tooltip("Case sensitive key.")]
        public FsmString[] keys;

        [UIHint(UIHint.Variable)]
        public FsmFloat[] variables;

        public override void Reset()
        {
            this.keys = new FsmString[1];
            this.variables = new FsmFloat[1];
        }

        public override void OnEnter()
        {
            for (int i = 0; i < this.keys.Length; i++)
            {
                if (!this.keys[i].IsNone || !this.keys[i].Value.Equals(""))
                {
                    this.variables[i].Value = PlayerPrefs.GetFloat(this.keys[i].Value, this.variables[i].IsNone ? 0f : this.variables[i].Value);
                }
            }
            base.Finish();
        }
    }
}
