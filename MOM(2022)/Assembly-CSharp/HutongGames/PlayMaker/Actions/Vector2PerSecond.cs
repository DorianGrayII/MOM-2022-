namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Vector2), HutongGames.PlayMaker.Tooltip("Multiplies a Vector2 variable by Time.deltaTime. Useful for frame rate independent motion.")]
    public class Vector2PerSecond : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The Vector2")]
        public FsmVector2 vector2Variable;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
        public bool everyFrame;

        public override void OnEnter()
        {
            this.vector2Variable.set_Value(this.vector2Variable.get_Value() * Time.deltaTime);
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.vector2Variable.set_Value(this.vector2Variable.get_Value() * Time.deltaTime);
        }

        public override void Reset()
        {
            this.vector2Variable = null;
            this.everyFrame = true;
        }
    }
}

