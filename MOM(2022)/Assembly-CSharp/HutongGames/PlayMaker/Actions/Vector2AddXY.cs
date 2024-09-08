using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Vector2)]
    [Tooltip("Adds a XY values to Vector2 Variable.")]
    public class Vector2AddXY : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The vector2 target")]
        public FsmVector2 vector2Variable;

        [Tooltip("The x component to add")]
        public FsmFloat addX;

        [Tooltip("The y component to add")]
        public FsmFloat addY;

        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        [Tooltip("Add the value on a per second bases.")]
        public bool perSecond;

        public override void Reset()
        {
            this.vector2Variable = null;
            this.addX = 0f;
            this.addY = 0f;
            this.everyFrame = false;
            this.perSecond = false;
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

        private void DoVector2AddXYZ()
        {
            Vector2 vector = new Vector2(this.addX.Value, this.addY.Value);
            if (this.perSecond)
            {
                this.vector2Variable.Value += vector * Time.deltaTime;
            }
            else
            {
                this.vector2Variable.Value += vector;
            }
        }
    }
}
