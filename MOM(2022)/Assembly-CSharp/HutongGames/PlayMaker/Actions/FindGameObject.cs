using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Finds a Game Object by Name and/or Tag.")]
    public class FindGameObject : FsmStateAction
    {
        [Tooltip("The name of the GameObject to find. You can leave this empty if you specify a Tag.")]
        public FsmString objectName;

        [UIHint(UIHint.Tag)]
        [Tooltip("Find a GameObject with this tag. If Object Name is specified then both name and Tag must match.")]
        public FsmString withTag;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a GameObject variable.")]
        public FsmGameObject store;

        public override void Reset()
        {
            this.objectName = "";
            this.withTag = "Untagged";
            this.store = null;
        }

        public override void OnEnter()
        {
            this.Find();
            base.Finish();
        }

        private void Find()
        {
            if (this.withTag.Value != "Untagged")
            {
                if (!string.IsNullOrEmpty(this.objectName.Value))
                {
                    GameObject[] array = GameObject.FindGameObjectsWithTag(this.withTag.Value);
                    foreach (GameObject gameObject in array)
                    {
                        if (gameObject.name == this.objectName.Value)
                        {
                            this.store.Value = gameObject;
                            return;
                        }
                    }
                    this.store.Value = null;
                }
                else
                {
                    this.store.Value = GameObject.FindGameObjectWithTag(this.withTag.Value);
                }
            }
            else
            {
                this.store.Value = GameObject.Find(this.objectName.Value);
            }
        }

        public override string ErrorCheck()
        {
            if (string.IsNullOrEmpty(this.objectName.Value) && string.IsNullOrEmpty(this.withTag.Value))
            {
                return "Specify Name, Tag, or both.";
            }
            return null;
        }
    }
}
