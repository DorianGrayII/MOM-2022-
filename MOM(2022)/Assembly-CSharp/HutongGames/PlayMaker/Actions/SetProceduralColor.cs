using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Substance")]
    [Tooltip("Set a named color property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
    public class SetProceduralColor : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The Substance Material.")]
        public FsmMaterial substanceMaterial;

        [RequiredField]
        [Tooltip("The named color property in the material.")]
        public FsmString colorProperty;

        [RequiredField]
        [Tooltip("The value to set the property to.")]
        public FsmColor colorValue;

        [Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
        public bool everyFrame;

        public override void Reset()
        {
            this.substanceMaterial = null;
            this.colorProperty = "";
            this.colorValue = Color.white;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetProceduralFloat();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetProceduralFloat();
        }

        private void DoSetProceduralFloat()
        {
        }
    }
}
