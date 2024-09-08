namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Vector2), Tooltip("Get Vector2 Length.")]
    public class GetVector2Length : FsmStateAction
    {
        [Tooltip("The Vector2 to get the length from")]
        public FsmVector2 vector2;
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The Vector2 the length")]
        public FsmFloat storeLength;
        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        private void DoVectorLength()
        {
            if ((this.vector2 != null) && (this.storeLength != null))
            {
                this.storeLength.Value = this.vector2.get_Value().magnitude;
            }
        }

        public override void OnEnter()
        {
            this.DoVectorLength();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoVectorLength();
        }

        public override void Reset()
        {
            this.vector2 = null;
            this.storeLength = null;
            this.everyFrame = false;
        }
    }
}

