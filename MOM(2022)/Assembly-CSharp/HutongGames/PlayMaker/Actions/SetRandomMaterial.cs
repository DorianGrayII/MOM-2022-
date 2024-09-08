using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Material)]
    [Tooltip("Sets a Game Object's material randomly from an array of Materials.")]
    public class SetRandomMaterial : ComponentAction<Renderer>
    {
        [RequiredField]
        [CheckForComponent(typeof(Renderer))]
        public FsmOwnerDefault gameObject;

        public FsmInt materialIndex;

        public FsmMaterial[] materials;

        public override void Reset()
        {
            this.gameObject = null;
            this.materialIndex = 0;
            this.materials = new FsmMaterial[3];
        }

        public override void OnEnter()
        {
            this.DoSetRandomMaterial();
            base.Finish();
        }

        private void DoSetRandomMaterial()
        {
            if (this.materials == null || this.materials.Length == 0)
            {
                return;
            }
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                if (base.renderer.material == null)
                {
                    base.LogError("Missing Material!");
                }
                else if (this.materialIndex.Value == 0)
                {
                    base.renderer.material = this.materials[Random.Range(0, this.materials.Length)].Value;
                }
                else if (base.renderer.materials.Length > this.materialIndex.Value)
                {
                    Material[] array = base.renderer.materials;
                    array[this.materialIndex.Value] = this.materials[Random.Range(0, this.materials.Length)].Value;
                    base.renderer.materials = array;
                }
            }
        }
    }
}
