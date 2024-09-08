namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Gets the pressed state of the specified Button and stores it in a Bool Variable. See Unity Input Manager docs.")]
    public class GetButton : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The name of the button. Set in the Unity Input Manager.")]
        public FsmString buttonName;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the result in a bool variable.")]
        public FsmBool storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoGetButton()
        {
            this.storeResult.Value = Input.GetButton(this.buttonName.Value);
        }

        public override void OnEnter()
        {
            this.DoGetButton();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetButton();
        }

        public override void Reset()
        {
            this.buttonName = "Fire1";
            this.storeResult = null;
            this.everyFrame = true;
        }
    }
}

