namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Finds a Game Object by Name and/or Tag.")]
    public class FindGameObject : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The name of the GameObject to find. You can leave this empty if you specify a Tag.")]
        public FsmString objectName;
        [UIHint(UIHint.Tag), HutongGames.PlayMaker.Tooltip("Find a GameObject with this tag. If Object Name is specified then both name and Tag must match.")]
        public FsmString withTag;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the result in a GameObject variable.")]
        public FsmGameObject store;

        public override string ErrorCheck()
        {
            return ((!string.IsNullOrEmpty(this.objectName.Value) || !string.IsNullOrEmpty(this.withTag.Value)) ? null : "Specify Name, Tag, or both.");
        }

        private void Find()
        {
            if (this.withTag.Value == "Untagged")
            {
                this.store.set_Value(GameObject.Find(this.objectName.Value));
            }
            else if (string.IsNullOrEmpty(this.objectName.Value))
            {
                this.store.set_Value(GameObject.FindGameObjectWithTag(this.withTag.Value));
            }
            else
            {
                foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag(this.withTag.Value))
                {
                    if (obj2.name == this.objectName.Value)
                    {
                        this.store.set_Value(obj2);
                        return;
                    }
                }
                this.store.set_Value((GameObject) null);
            }
        }

        public override void OnEnter()
        {
            this.Find();
            base.Finish();
        }

        public override void Reset()
        {
            this.objectName = "";
            this.withTag = "Untagged";
            this.store = null;
        }
    }
}

