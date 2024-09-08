using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Math)]
    [Tooltip("Sets a Bool Variable to True or False randomly.")]
    public class RandomBool : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        public FsmBool storeResult;

        public override void Reset()
        {
            this.storeResult = null;
        }

        public override void OnEnter()
        {
            this.storeResult.Value = Random.Range(0, 100) < 50;
            base.Finish();
        }
    }
}
