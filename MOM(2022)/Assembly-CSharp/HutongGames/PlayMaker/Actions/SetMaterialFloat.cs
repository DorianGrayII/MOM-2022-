﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Material), HutongGames.PlayMaker.Tooltip("Sets a named float in a game object's material.")]
    public class SetMaterialFloat : ComponentAction<Renderer>
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject that the material is applied to."), CheckForComponent(typeof(Renderer))]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
        public FsmInt materialIndex;
        [HutongGames.PlayMaker.Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
        public FsmMaterial material;
        [RequiredField, HutongGames.PlayMaker.Tooltip("A named float parameter in the shader.")]
        public FsmString namedFloat;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Set the parameter value.")]
        public FsmFloat floatValue;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is animated.")]
        public bool everyFrame;

        private void DoSetMaterialFloat()
        {
            if (this.material.get_Value() != null)
            {
                this.material.get_Value().SetFloat(this.namedFloat.Value, this.floatValue.Value);
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
                        base.renderer.material.SetFloat(this.namedFloat.Value, this.floatValue.Value);
                    }
                    else if (base.renderer.materials.Length > this.materialIndex.Value)
                    {
                        Material[] materials = base.renderer.materials;
                        materials[this.materialIndex.Value].SetFloat(this.namedFloat.Value, this.floatValue.Value);
                        base.renderer.materials = materials;
                    }
                }
            }
        }

        public override void OnEnter()
        {
            this.DoSetMaterialFloat();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetMaterialFloat();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.materialIndex = 0;
            this.material = null;
            this.namedFloat = "";
            this.floatValue = 0f;
            this.everyFrame = false;
        }
    }
}

