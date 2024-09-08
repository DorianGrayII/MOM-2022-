namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Interrupts the automatic target matching. CompleteMatch will make the gameobject match the target completely at the next frame.")]
    public class AnimatorInterruptMatchTarget : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Will make the gameobject match the target completely at the next frame")]
        public FsmBool completeMatch;

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                base.Finish();
            }
            else
            {
                Animator component = ownerDefaultTarget.GetComponent<Animator>();
                if (component != null)
                {
                    component.InterruptMatchTarget(this.completeMatch.Value);
                }
                base.Finish();
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.completeMatch = true;
        }
    }
}

