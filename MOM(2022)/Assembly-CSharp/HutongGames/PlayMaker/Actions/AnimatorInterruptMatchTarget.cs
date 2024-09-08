using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Interrupts the automatic target matching. CompleteMatch will make the gameobject match the target completely at the next frame.")]
    public class AnimatorInterruptMatchTarget : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Will make the gameobject match the target completely at the next frame")]
        public FsmBool completeMatch;

        public override void Reset()
        {
            this.gameObject = null;
            this.completeMatch = true;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                base.Finish();
                return;
            }
            Animator component = ownerDefaultTarget.GetComponent<Animator>();
            if (component != null)
            {
                component.InterruptMatchTarget(this.completeMatch.Value);
            }
            base.Finish();
        }
    }
}
