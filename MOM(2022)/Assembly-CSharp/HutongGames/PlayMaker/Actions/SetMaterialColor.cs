namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Material), HutongGames.PlayMaker.Tooltip("Sets a named color value in a game object's material.")]
    public class SetMaterialColor : ComponentAction<Renderer>
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject that the material is applied to."), CheckForComponent(typeof(Renderer))]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
        public FsmInt materialIndex;
        [HutongGames.PlayMaker.Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
        public FsmMaterial material;
        [UIHint(UIHint.NamedColor), HutongGames.PlayMaker.Tooltip("A named color parameter in the shader.")]
        public FsmString namedColor;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Set the parameter value.")]
        public FsmColor color;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is animated.")]
        public bool everyFrame;

        private void DoSetMaterialColor()
        {
            if (!this.color.IsNone)
            {
                string name = this.namedColor.Value;
                if (name == "")
                {
                    name = "_Color";
                }
                if (this.material.get_Value() != null)
                {
                    this.material.get_Value().SetColor(name, this.color.get_Value());
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
                            base.renderer.material.SetColor(name, this.color.get_Value());
                        }
                        else if (base.renderer.materials.Length > this.materialIndex.Value)
                        {
                            Material[] materials = base.renderer.materials;
                            materials[this.materialIndex.Value].SetColor(name, this.color.get_Value());
                            base.renderer.materials = materials;
                        }
                    }
                }
            }
        }

        public override void OnEnter()
        {
            this.DoSetMaterialColor();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetMaterialColor();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.materialIndex = 0;
            this.material = null;
            this.namedColor = "_Color";
            this.color = (FsmColor) Color.black;
            this.everyFrame = false;
        }
    }
}

