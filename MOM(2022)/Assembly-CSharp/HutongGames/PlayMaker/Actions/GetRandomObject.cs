using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Gets a Random Game Object from the scene.\nOptionally filter by Tag.")]
    public class GetRandomObject : FsmStateAction
    {
        [UIHint(UIHint.Tag)]
        public FsmString withTag;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmGameObject storeResult;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.withTag = "Untagged";
            this.storeResult = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoGetRandomObject();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetRandomObject();
        }

        private void DoGetRandomObject()
        {
            GameObject[] array = ((!(this.withTag.Value != "Untagged")) ? ((GameObject[])Object.FindObjectsOfType(typeof(GameObject))) : GameObject.FindGameObjectsWithTag(this.withTag.Value));
            if (array.Length != 0)
            {
                this.storeResult.Value = array[Random.Range(0, array.Length)];
            }
            else
            {
                this.storeResult.Value = null;
            }
        }
    }
}
