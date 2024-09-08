namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Vector3), HutongGames.PlayMaker.Tooltip("Adds a value to Vector3 Variable.")]
    public class Vector3Add : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmVector3 vector3Variable;
        [RequiredField]
        public FsmVector3 addVector;
        public bool everyFrame;
        public bool perSecond;

        private void DoVector3Add()
        {
            if (this.perSecond)
            {
                this.vector3Variable.set_Value(this.vector3Variable.get_Value() + (this.addVector.get_Value() * Time.deltaTime));
            }
            else
            {
                this.vector3Variable.set_Value(this.vector3Variable.get_Value() + this.addVector.get_Value());
            }
        }

        public override void OnEnter()
        {
            this.DoVector3Add();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoVector3Add();
        }

        public override void Reset()
        {
            this.vector3Variable = null;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.addVector = vector1;
            this.everyFrame = false;
            this.perSecond = false;
        }
    }
}

