namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Gets the number of Game Objects in the scene with the specified Tag.")]
    public class GetTagCount : FsmStateAction
    {
        [UIHint(UIHint.Tag)]
        public FsmString tag;
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmInt storeResult;

        public override void OnEnter()
        {
            GameObject[] objArray = GameObject.FindGameObjectsWithTag(this.tag.Value);
            if (this.storeResult != null)
            {
                this.storeResult.Value = (objArray != null) ? objArray.Length : 0;
            }
            base.Finish();
        }

        public override void Reset()
        {
            this.tag = "Untagged";
            this.storeResult = null;
        }
    }
}

