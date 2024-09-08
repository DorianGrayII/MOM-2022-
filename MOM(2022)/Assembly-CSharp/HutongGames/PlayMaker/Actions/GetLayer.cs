namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.GameObject), Tooltip("Gets a Game Object's Layer and stores it in an Int Variable.")]
    public class GetLayer : FsmStateAction
    {
        [RequiredField]
        public FsmGameObject gameObject;
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmInt storeResult;
        public bool everyFrame;

        private void DoGetLayer()
        {
            if (this.gameObject.get_Value() != null)
            {
                this.storeResult.Value = this.gameObject.get_Value().layer;
            }
        }

        public override void OnEnter()
        {
            this.DoGetLayer();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetLayer();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.storeResult = null;
            this.everyFrame = false;
        }
    }
}

