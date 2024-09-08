namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Vector2), HutongGames.PlayMaker.Tooltip("Adds a XY values to Vector2 Variable.")]
    public class Vector2AddXY : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The vector2 target")]
        public FsmVector2 vector2Variable;
        [HutongGames.PlayMaker.Tooltip("The x component to add")]
        public FsmFloat addX;
        [HutongGames.PlayMaker.Tooltip("The y component to add")]
        public FsmFloat addY;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
        public bool everyFrame;
        [HutongGames.PlayMaker.Tooltip("Add the value on a per second bases.")]
        public bool perSecond;

        private void DoVector2AddXYZ()
        {
            Vector2 vector = new Vector2(this.addX.Value, this.addY.Value);
            if (this.perSecond)
            {
                this.vector2Variable.set_Value(this.vector2Variable.get_Value() + (vector * Time.deltaTime));
            }
            else
            {
                this.vector2Variable.set_Value(this.vector2Variable.get_Value() + vector);
            }
        }

        public override void OnEnter()
        {
            this.DoVector2AddXYZ();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoVector2AddXYZ();
        }

        public override void Reset()
        {
            this.vector2Variable = null;
            this.addX = 0f;
            this.addY = 0f;
            this.everyFrame = false;
            this.perSecond = false;
        }
    }
}

