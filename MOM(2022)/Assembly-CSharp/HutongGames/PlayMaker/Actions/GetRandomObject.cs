namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Gets a Random Game Object from the scene.\nOptionally filter by Tag.")]
    public class GetRandomObject : FsmStateAction
    {
        [UIHint(UIHint.Tag)]
        public FsmString withTag;
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmGameObject storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoGetRandomObject()
        {
            GameObject[] objArray = (this.withTag.Value == "Untagged") ? ((GameObject[]) UnityEngine.Object.FindObjectsOfType(typeof(GameObject))) : GameObject.FindGameObjectsWithTag(this.withTag.Value);
            if (objArray.Length != 0)
            {
                this.storeResult.set_Value(objArray[UnityEngine.Random.Range(0, objArray.Length)]);
            }
            else
            {
                this.storeResult.set_Value((GameObject) null);
            }
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

        public override void Reset()
        {
            this.withTag = "Untagged";
            this.storeResult = null;
            this.everyFrame = false;
        }
    }
}

