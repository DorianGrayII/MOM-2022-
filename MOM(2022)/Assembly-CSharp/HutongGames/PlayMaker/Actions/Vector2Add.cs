using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Vector2)]
    [Tooltip("Adds a value to Vector2 Variable.")]
    public class Vector2Add : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The vector2 target")]
        public FsmVector2 vector2Variable;

        [RequiredField]
        [Tooltip("The vector2 to add")]
        public FsmVector2 addVector;

        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        [Tooltip("Add the value on a per second bases.")]
        public bool perSecond;

        public override void Reset()
        {
            this.vector2Variable = null;
            this.addVector = new FsmVector2
            {
                UseVariable = true
            };
            this.everyFrame = false;
            this.perSecond = false;
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

        private void DoVector2Add()
        {
            if (this.perSecond)
            {
                this.vector2Variable.Value = this.vector2Variable.Value + this.addVector.Value * Time.deltaTime;
            }
            else
            {
                this.vector2Variable.Value = this.vector2Variable.Value + this.addVector.Value;
            }
        }
    }
}
