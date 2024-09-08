using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Finds the Child of a GameObject by Name and/or Tag. Use this to find attach points etc. NOTE: This action will search recursively through all children and return the first match; To find a specific child use Find Child.")]
    public class GetChild : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject to search.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The name of the child to search for.")]
        public FsmString childName;

        [UIHint(UIHint.Tag)]
        [Tooltip("The Tag to search for. If Child Name is set, both name and Tag need to match.")]
        public FsmString withTag;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a GameObject variable.")]
        public FsmGameObject storeResult;

        public override void Reset()
        {
            this.gameObject = null;
            this.childName = "";
            this.withTag = "Untagged";
            this.storeResult = null;
        }

        public override void OnEnter()
        {
            this.storeResult.Value = GetChild.DoGetChildByName(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.childName.Value, this.withTag.Value);
            base.Finish();
        }

        private static GameObject DoGetChildByName(GameObject root, string name, string tag)
        {
            if (root == null)
            {
                return null;
            }
            foreach (Transform item in root.transform)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (item.name == name)
                    {
                        if (string.IsNullOrEmpty(tag))
                        {
                            return item.gameObject;
                        }
                        if (item.tag.Equals(tag))
                        {
                            return item.gameObject;
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(tag) && item.tag == tag)
                {
                    return item.gameObject;
                }
                GameObject gameObject = GetChild.DoGetChildByName(item.gameObject, name, tag);
                if (gameObject != null)
                {
                    return gameObject;
                }
            }
            return null;
        }

        public override string ErrorCheck()
        {
            if (string.IsNullOrEmpty(this.childName.Value) && string.IsNullOrEmpty(this.withTag.Value))
            {
                return "Specify Child Name, Tag, or both.";
            }
            return null;
        }
    }
}
