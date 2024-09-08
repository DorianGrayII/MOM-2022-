namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Material), HutongGames.PlayMaker.Tooltip("Sets a named texture in a game object's material.")]
    public class SetMaterialTexture : ComponentAction<Renderer>
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject that the material is applied to."), CheckForComponent(typeof(Renderer))]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
        public FsmInt materialIndex;
        [HutongGames.PlayMaker.Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
        public FsmMaterial material;
        [UIHint(UIHint.NamedTexture), HutongGames.PlayMaker.Tooltip("A named parameter in the shader.")]
        public FsmString namedTexture;
        public FsmTexture texture;

        private void DoSetMaterialTexture()
        {
            string name = this.namedTexture.Value;
            if (name == "")
            {
                name = "_MainTex";
            }
            if (this.material.get_Value() != null)
            {
                this.material.get_Value().SetTexture(name, this.texture.get_Value());
            }
            else
            {
                GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
                if (base.UpdateCache(ownerDefaultTarget))
                {
                    if (base.renderer.material == null)
                    {
                        base.LogError("Missing Material!");
                    }
                    else if (this.materialIndex.Value == 0)
                    {
                        base.renderer.material.SetTexture(name, this.texture.get_Value());
                    }
                    else if (base.renderer.materials.Length > this.materialIndex.Value)
                    {
                        Material[] materials = base.renderer.materials;
                        materials[this.materialIndex.Value].SetTexture(name, this.texture.get_Value());
                        base.renderer.materials = materials;
                    }
                }
            }
        }

        public override void OnEnter()
        {
            this.DoSetMaterialTexture();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.materialIndex = 0;
            this.material = null;
            this.namedTexture = "_MainTex";
            this.texture = null;
        }
    }
}

