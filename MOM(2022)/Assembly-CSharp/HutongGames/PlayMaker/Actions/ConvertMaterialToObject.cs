namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Convert), Tooltip("Converts a Material variable to an Object variable. Useful if you want to use Set Property (which only works on Object variables).")]
    public class ConvertMaterialToObject : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The Material variable to convert to an Object.")]
        public FsmMaterial materialVariable;
        [RequiredField, UIHint(UIHint.Variable), Tooltip("Store the result in an Object variable.")]
        public FsmObject objectVariable;
        [Tooltip("Repeat every frame. Useful if the Material variable is changing.")]
        public bool everyFrame;

        private void DoConvertMaterialToObject()
        {
            this.objectVariable.set_Value(this.materialVariable.get_Value());
        }

        public override void OnEnter()
        {
            this.DoConvertMaterialToObject();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoConvertMaterialToObject();
        }

        public override void Reset()
        {
            this.materialVariable = null;
            this.objectVariable = null;
            this.everyFrame = false;
        }
    }
}

