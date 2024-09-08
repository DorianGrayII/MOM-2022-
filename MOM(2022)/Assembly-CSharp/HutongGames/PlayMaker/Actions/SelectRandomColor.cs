namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Color), Tooltip("Select a random Color from an array of Colors.")]
    public class SelectRandomColor : FsmStateAction
    {
        [CompoundArray("Colors", "Color", "Weight")]
        public FsmColor[] colors;
        [HasFloatSlider(0f, 1f)]
        public FsmFloat[] weights;
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmColor storeColor;

        private void DoSelectRandomColor()
        {
            if (((this.colors != null) && (this.colors.Length != 0)) && (this.storeColor != null))
            {
                int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
                if (randomWeightedIndex != -1)
                {
                    this.storeColor.set_Value(this.colors[randomWeightedIndex].get_Value());
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
            this.colors = new FsmColor[3];
            this.weights = new FsmFloat[] { 1f, 1f, 1f };
            this.storeColor = null;
        }
    }
}

