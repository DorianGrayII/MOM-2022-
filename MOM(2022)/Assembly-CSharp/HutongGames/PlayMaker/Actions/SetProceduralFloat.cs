namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory("Substance"), Tooltip("Set a named float property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
    public class SetProceduralFloat : FsmStateAction
    {
        [RequiredField, Tooltip("The Substance Material.")]
        public FsmMaterial substanceMaterial;
        [RequiredField, Tooltip("The named float property in the material.")]
        public FsmString floatProperty;
        [RequiredField, Tooltip("The value to set the property to.")]
        public FsmFloat floatValue;
        [Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
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
            this.floatProperty = "";
            this.floatValue = 0f;
            this.everyFrame = false;
        }
    }
}

