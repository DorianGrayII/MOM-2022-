using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Vector2)]
    [Tooltip("Multiplies a Vector2 variable by Time.deltaTime. Useful for frame rate independent motion.")]
    public class Vector2PerSecond : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The Vector2")]
        public FsmVector2 vector2Variable;

        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        public override void Reset()
        {
            this.vector2Variable = null;
            this.everyFrame = true;
        }

        public override void OnEnter()
        {
            this.vector2Variable.Value = this.vector2Variable.Value * Time.deltaTime;
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.vector2Variable.Value = this.vector2Variable.Value * Time.deltaTime;
        }
    }
}
