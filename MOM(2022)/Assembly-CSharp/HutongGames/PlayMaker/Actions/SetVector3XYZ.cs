namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Vector3), HutongGames.PlayMaker.Tooltip("Sets the XYZ channels of a Vector3 Variable. To leave any channel unchanged, set variable to 'None'.")]
    public class SetVector3XYZ : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmVector3 vector3Variable;
        [UIHint(UIHint.Variable)]
        public FsmVector3 vector3Value;
        public FsmFloat x;
        public FsmFloat y;
        public FsmFloat z;
        public bool everyFrame;

        private void DoSetVector3XYZ()
        {
            if (this.vector3Variable != null)
            {
                Vector3 vector = this.vector3Variable.get_Value();
                if (!this.vector3Value.IsNone)
                {
                    vector = this.vector3Value.get_Value();
                }
                if (!this.x.IsNone)
                {
                    vector.x = this.x.Value;
                }
                if (!this.y.IsNone)
                {
                    vector.y = this.y.Value;
                }
                if (!this.z.IsNone)
                {
                    vector.z = this.z.Value;
                }
                this.vector3Variable.set_Value(vector);
            }
        }

        public override void OnEnter()
        {
            this.DoSetVector3XYZ();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetVector3XYZ();
        }

        public override void Reset()
        {
            this.vector3Variable = null;
            this.vector3Value = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.x = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.y = num2;
            FsmFloat num3 = new FsmFloat();
            num3.UseVariable = true;
            this.z = num3;
            this.everyFrame = false;
        }
    }
}

