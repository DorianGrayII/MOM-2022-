namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.GameObject), Tooltip("Gets a Game Object's Tag and stores it in a String Variable.")]
    public class GetTag : FsmStateAction
    {
        [RequiredField]
        public FsmGameObject gameObject;
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmString storeResult;
        public bool everyFrame;

        private void DoGetTag()
        {
            if (this.gameObject.get_Value() != null)
            {
                this.storeResult.Value = this.gameObject.get_Value().tag;
            }
        }

        public override void OnEnter()
        {
            this.DoGetTag();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetTag();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.storeResult = null;
            this.everyFrame = false;
        }
    }
}

