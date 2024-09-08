namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Vector2), HutongGames.PlayMaker.Tooltip("Use a low pass filter to reduce the influence of sudden changes in a Vector2 Variable.")]
    public class Vector2LowPassFilter : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Vector2 Variable to filter. Should generally come from some constantly updated input")]
        public FsmVector2 vector2Variable;
        [HutongGames.PlayMaker.Tooltip("Determines how much influence new changes have. E.g., 0.1 keeps 10 percent of the unfiltered vector and 90 percent of the previously filtered value")]
        public FsmFloat filteringFactor;
        private Vector2 filteredVector;

        public override void OnEnter()
        {
            this.filteredVector = new Vector2(this.vector2Variable.get_Value().x, this.vector2Variable.get_Value().y);
        }

        public override void OnUpdate()
        {
            this.filteredVector.x = (this.vector2Variable.get_Value().x * this.filteringFactor.Value) + (this.filteredVector.x * (1f - this.filteringFactor.Value));
            this.filteredVector.y = (this.vector2Variable.get_Value().y * this.filteringFactor.Value) + (this.filteredVector.y * (1f - this.filteringFactor.Value));
            this.vector2Variable.set_Value(new Vector2(this.filteredVector.x, this.filteredVector.y));
        }

        public override void Reset()
        {
            this.vector2Variable = null;
            this.filteringFactor = 0.1f;
        }
    }
}

