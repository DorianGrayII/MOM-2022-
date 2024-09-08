namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using System.Collections;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Finds the Child of a GameObject by Name and/or Tag. Use this to find attach points etc. NOTE: This action will search recursively through all children and return the first match; To find a specific child use Find Child.")]
    public class GetChild : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to search.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The name of the child to search for.")]
        public FsmString childName;
        [UIHint(UIHint.Tag), HutongGames.PlayMaker.Tooltip("The Tag to search for. If Child Name is set, both name and Tag need to match.")]
        public FsmString withTag;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the result in a GameObject variable.")]
        public FsmGameObject storeResult;

        private static GameObject DoGetChildByName(GameObject root, string name, string tag)
        {
            GameObject gameObject;
            if (root == null)
            {
                return null;
            }
            using (IEnumerator enumerator = root.transform.GetEnumerator())
            {
                while (true)
                {
                    if (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        if (string.IsNullOrEmpty(name))
                        {
                            if (!string.IsNullOrEmpty(tag) && (current.tag == tag))
                            {
                                gameObject = current.gameObject;
                                break;
                            }
                        }
                        else if (current.name == name)
                        {
                            if (string.IsNullOrEmpty(tag))
                            {
                                gameObject = current.gameObject;
                                break;
                            }
                            if (current.tag.Equals(tag))
                            {
                                gameObject = current.gameObject;
                                break;
                            }
                        }
                        GameObject obj2 = DoGetChildByName(current.gameObject, name, tag);
                        if (obj2 == null)
                        {
                            continue;
                        }
                        gameObject = obj2;
                    }
                    else
                    {
                        return null;
                    }
                    break;
                }
            }
            return gameObject;
        }

        public override string ErrorCheck()
        {
            return ((!string.IsNullOrEmpty(this.childName.Value) || !string.IsNullOrEmpty(this.withTag.Value)) ? null : "Specify Child Name, Tag, or both.");
        }

        public override void OnEnter()
        {
            this.storeResult.set_Value(DoGetChildByName(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.childName.Value, this.withTag.Value));
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.childName = "";
            this.withTag = "Untagged";
            this.storeResult = null;
        }
    }
}

