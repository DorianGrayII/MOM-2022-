namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Math), HutongGames.PlayMaker.Tooltip("Adds a value to a Float Variable.")]
    public class FloatAdd : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The Float variable to add to.")]
        public FsmFloat floatVariable;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Amount to add.")]
        public FsmFloat add;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;
        [HutongGames.PlayMaker.Tooltip("Used with Every Frame. Adds the value over one second to make the operation frame rate independent.")]
        public bool perSecond;

        private void DoFloatAdd()
        {
            if (!this.perSecond)
            {
                this.floatVariable.Value += this.add.Value;
            }
            else
            {
                this.floatVariable.Value += this.add.Value * Time.deltaTime;
            }
        }

        public override void OnEnter()
        {
            this.DoFloatAdd();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoFloatAdd();
        }

        public override void Reset()
        {
            this.floatVariable = null;
            this.add = null;
            this.everyFrame = false;
            this.perSecond = false;
        }
    }
}

