namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Level), HutongGames.PlayMaker.Tooltip("Makes the Game Object not be destroyed automatically when loading a new scene.")]
    public class DontDestroyOnLoad : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("GameObject to mark as DontDestroyOnLoad.")]
        public FsmOwnerDefault gameObject;

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                UnityEngine.Object.DontDestroyOnLoad(ownerDefaultTarget.transform.root.gameObject);
            }
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
        }
    }
}

