namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Substance")]
    [Tooltip("Set a named Vector3 property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
    public class SetProceduralVector3 : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The Substance Material.")]
        public FsmMaterial substanceMaterial;

        [RequiredField]
        [Tooltip("The named vector property in the material.")]
        public FsmString vector3Property;

        [RequiredField]
        [Tooltip("The value to set the property to.\nNOTE: Use Set Procedural Vector3 for Vector3 values.")]
        public FsmVector3 vector3Value;

        [Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
        public bool everyFrame;

        public override void Reset()
        {
            this.substanceMaterial = null;
            this.vector3Property = null;
            this.vector3Value = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetProceduralVector();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetProceduralVector();
        }

        private void DoSetProceduralVector()
        {
        }
    }
}
