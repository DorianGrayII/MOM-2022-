namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Material), HutongGames.PlayMaker.Tooltip("Sets the material on a game object.")]
    public class SetMaterial : ComponentAction<Renderer>
    {
        [RequiredField, CheckForComponent(typeof(Renderer))]
        public FsmOwnerDefault gameObject;
        public FsmInt materialIndex;
        [RequiredField]
        public FsmMaterial material;

        private void DoSetMaterial()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                if (this.materialIndex.Value == 0)
                {
                    base.renderer.material = this.material.get_Value();
                }
                else if (base.renderer.materials.Length > this.materialIndex.Value)
                {
                    Material[] materials = base.renderer.materials;
                    materials[this.materialIndex.Value] = this.material.get_Value();
                    base.renderer.materials = materials;
                }
            }
        }

        public override void OnEnter()
        {
            this.DoSetMaterial();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.material = null;
            this.materialIndex = 0;
        }
    }
}

