namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Logic), Tooltip("Tests if all the given Bool Variables are True.")]
    public class BoolAllTrue : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Readonly, Tooltip("The Bool variables to check.")]
        public FsmBool[] boolVariables;
        [Tooltip("Event to send if all the Bool variables are True.")]
        public FsmEvent sendEvent;
        [UIHint(UIHint.Variable), Tooltip("Store the result in a Bool variable.")]
        public FsmBool storeResult;
        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        private void DoAllTrue()
        {
            if (this.boolVariables.Length != 0)
            {
                bool flag = true;
                int index = 0;
                while (true)
                {
                    if (index < this.boolVariables.Length)
                    {
                        if (this.boolVariables[index].Value)
                        {
                            index++;
                            continue;
                        }
                        flag = false;
                    }
                    if (flag)
                    {
                        base.Fsm.Event(this.sendEvent);
                    }
                    this.storeResult.Value = flag;
                    return;
                }
            }
        }

        public override void OnEnter()
        {
            this.DoAllTrue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoAllTrue();
        }

        public override void Reset()
        {
            this.boolVariables = null;
            this.sendEvent = null;
            this.storeResult = null;
            this.everyFrame = false;
        }
    }
}

