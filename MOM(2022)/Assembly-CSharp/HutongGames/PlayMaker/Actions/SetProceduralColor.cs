namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("Substance"), HutongGames.PlayMaker.Tooltip("Set a named color property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
    public class SetProceduralColor : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The Substance Material.")]
        public FsmMaterial substanceMaterial;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The named color property in the material.")]
        public FsmString colorProperty;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The value to set the property to.")]
        public FsmColor colorValue;
        [HutongGames.PlayMaker.Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
        public bool everyFrame;

        private void DoSetProceduralFloat()
        {
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

        public override void Reset()
        {
            this.substanceMaterial = null;
            this.colorProperty = "";
            this.colorValue = (FsmColor) Color.white;
            this.everyFrame = false;
        }
    }
}

