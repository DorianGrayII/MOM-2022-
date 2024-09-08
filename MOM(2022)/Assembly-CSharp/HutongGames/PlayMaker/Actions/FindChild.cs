namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Finds the Child of a GameObject by Name.\nNote, you can specify a path to the child, e.g., LeftShoulder/Arm/Hand/Finger. If you need to specify a tag, use GetChild.")]
    public class FindChild : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to search.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The name of the child. Note, you can specify a path to the child, e.g., LeftShoulder/Arm/Hand/Finger")]
        public FsmString childName;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the child in a GameObject variable.")]
        public FsmGameObject storeResult;

        private void DoFindChild()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                Transform transform = ownerDefaultTarget.transform.Find(this.childName.Value);
                this.storeResult.set_Value(transform?.gameObject);
            }
        }

        public override void OnEnter()
        {
            this.DoFindChild();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.childName = "";
            this.storeResult = null;
        }
    }
}

