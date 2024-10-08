using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Application)]
    [Tooltip("Gets the Height of the Screen in pixels.")]
    public class GetScreenHeight : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmFloat storeScreenHeight;

        public override void Reset()
        {
            this.storeScreenHeight = null;
        }

        public override void OnEnter()
        {
            this.storeScreenHeight.Value = Screen.height;
            base.Finish();
        }
    }
}
