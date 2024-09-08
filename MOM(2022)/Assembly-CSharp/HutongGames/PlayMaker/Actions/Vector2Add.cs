namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Vector2), HutongGames.PlayMaker.Tooltip("Adds a value to Vector2 Variable.")]
    public class Vector2Add : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The vector2 target")]
        public FsmVector2 vector2Variable;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The vector2 to add")]
        public FsmVector2 addVector;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
        public bool everyFrame;
        [HutongGames.PlayMaker.Tooltip("Add the value on a per second bases.")]
        public bool perSecond;

        private void DoVector2Add()
        {
            if (this.perSecond)
            {
                this.vector2Variable.set_Value(this.vector2Variable.get_Value() + (this.addVector.get_Value() * Time.deltaTime));
            }
            else
            {
                this.vector2Variable.set_Value(this.vector2Variable.get_Value() + this.addVector.get_Value());
            }
        }

        public override void OnEnter()
        {
            this.DoVector2Add();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoVector2Add();
        }

        public override void Reset()
        {
            this.vector2Variable = null;
            FsmVector2 vector1 = new FsmVector2();
            vector1.UseVariable = true;
            this.addVector = vector1;
            this.everyFrame = false;
            this.perSecond = false;
        }
    }
}

