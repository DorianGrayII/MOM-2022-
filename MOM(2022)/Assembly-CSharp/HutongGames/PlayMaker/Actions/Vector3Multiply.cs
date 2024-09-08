namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Vector3), Tooltip("Multiplies a Vector3 variable by a Float.")]
    public class Vector3Multiply : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmVector3 vector3Variable;
        [RequiredField]
        public FsmFloat multiplyBy;
        public bool everyFrame;

        public override void OnEnter()
        {
            this.vector3Variable.set_Value(this.vector3Variable.get_Value() * this.multiplyBy.Value);
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.vector3Variable.set_Value(this.vector3Variable.get_Value() * this.multiplyBy.Value);
        }

        public override void Reset()
        {
            this.vector3Variable = null;
            this.multiplyBy = 1f;
            this.everyFrame = false;
        }
    }
}

