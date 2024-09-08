using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Gets the number of Game Objects in the scene with the specified Tag.")]
    public class GetTagCount : FsmStateAction
    {
        [UIHint(UIHint.Tag)]
        public FsmString tag;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmInt storeResult;

        public override void Reset()
        {
            this.tag = "Untagged";
            this.storeResult = null;
        }

        public override void OnEnter()
        {
            GameObject[] array = GameObject.FindGameObjectsWithTag(this.tag.Value);
            if (this.storeResult != null)
            {
                this.storeResult.Value = ((array != null) ? array.Length : 0);
            }
            base.Finish();
        }
    }
}
