namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.UnityObject), Tooltip("Sets the value of an Object Variable.")]
    public class SetObjectValue : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmObject objectVariable;
        [RequiredField]
        public FsmObject objectValue;
        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void OnEnter()
        {
            this.objectVariable.set_Value(this.objectValue.get_Value());
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.objectVariable.set_Value(this.objectValue.get_Value());
        }

        public override void Reset()
        {
            this.objectVariable = null;
            this.objectValue = null;
            this.everyFrame = false;
        }
    }
}

