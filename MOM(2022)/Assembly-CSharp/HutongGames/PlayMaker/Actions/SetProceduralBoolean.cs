namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Substance")]
    [Tooltip("Set a named bool property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
    public class SetProceduralBoolean : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The Substance Material.")]
        public FsmMaterial substanceMaterial;

        [RequiredField]
        [Tooltip("The named bool property in the material.")]
        public FsmString boolProperty;

        [RequiredField]
        [Tooltip("The value to set the property to.")]
        public FsmBool boolValue;

        [Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
        public bool everyFrame;

        public override void Reset()
        {
            this.substanceMaterial = null;
            this.boolProperty = "";
            this.boolValue = false;
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
