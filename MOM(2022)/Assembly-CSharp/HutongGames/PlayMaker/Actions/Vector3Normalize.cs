namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Vector3), Tooltip("Normalizes a Vector3 Variable.")]
    public class Vector3Normalize : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmVector3 vector3Variable;
        public bool everyFrame;

        public override void OnEnter()
        {
            this.vector3Variable.set_Value(this.vector3Variable.get_Value().normalized);
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.vector3Variable.set_Value(this.vector3Variable.get_Value().normalized);
        }

        public override void Reset()
        {
            this.vector3Variable = null;
            this.everyFrame = false;
        }
    }
}

