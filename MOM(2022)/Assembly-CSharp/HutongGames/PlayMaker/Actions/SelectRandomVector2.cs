namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Vector2), Tooltip("Select a Random Vector2 from a Vector2 array.")]
    public class SelectRandomVector2 : FsmStateAction
    {
        [Tooltip("The array of Vectors and respective weights"), CompoundArray("Vectors", "Vector", "Weight")]
        public FsmVector2[] vector2Array;
        [HasFloatSlider(0f, 1f)]
        public FsmFloat[] weights;
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The picked vector2")]
        public FsmVector2 storeVector2;

        private void DoSelectRandomColor()
        {
            if (((this.vector2Array != null) && (this.vector2Array.Length != 0)) && (this.storeVector2 != null))
            {
                int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
                if (randomWeightedIndex != -1)
                {
                    this.storeVector2.set_Value(this.vector2Array[randomWeightedIndex].get_Value());
                }
            }
        }

        public override void OnEnter()
        {
            this.DoSelectRandomColor();
            base.Finish();
        }

        public override void Reset()
        {
            this.vector2Array = new FsmVector2[3];
            this.weights = new FsmFloat[] { 1f, 1f, 1f };
            this.storeVector2 = null;
        }
    }
}

