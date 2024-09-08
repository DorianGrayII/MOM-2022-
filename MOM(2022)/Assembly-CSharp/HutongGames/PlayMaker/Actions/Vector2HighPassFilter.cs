using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Vector2)]
    [Tooltip("Use a high pass filter to isolate sudden changes in a Vector2 Variable.")]
    public class Vector2HighPassFilter : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Vector2 Variable to filter. Should generally come from some constantly updated input.")]
        public FsmVector2 vector2Variable;

        [Tooltip("Determines how much influence new changes have.")]
        public FsmFloat filteringFactor;

        private Vector2 filteredVector;

        public override void Reset()
        {
            this.vector2Variable = null;
            this.filteringFactor = 0.1f;
        }

        public override void OnEnter()
        {
            this.filteredVector = new Vector2(this.vector2Variable.Value.x, this.vector2Variable.Value.y);
        }

        public override void OnUpdate()
        {
            this.filteredVector.x = this.vector2Variable.Value.x - (this.vector2Variable.Value.x * this.filteringFactor.Value + this.filteredVector.x * (1f - this.filteringFactor.Value));
            this.filteredVector.y = this.vector2Variable.Value.y - (this.vector2Variable.Value.y * this.filteringFactor.Value + this.filteredVector.y * (1f - this.filteringFactor.Value));
            this.vector2Variable.Value = new Vector2(this.filteredVector.x, this.filteredVector.y);
        }
    }
}
