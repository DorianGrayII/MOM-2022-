using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("Flips the horizontal and vertical axes of the RectTransform size and alignment, and optionally its children as well.")]
    public class RectTransformFlipLayoutAxis : FsmStateAction
    {
        public enum RectTransformFlipOptions
        {
            Horizontal = 0,
            Vertical = 1,
            Both = 2
        }

        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The axis to flip")]
        public RectTransformFlipOptions axis;

        [Tooltip("Flips around the pivot if true. Flips within the parent rect if false.")]
        public FsmBool keepPositioning;

        [Tooltip("Flip the children as well?")]
        public FsmBool recursive;

        public override void Reset()
        {
            this.gameObject = null;
            this.axis = RectTransformFlipOptions.Both;
            this.keepPositioning = null;
            this.recursive = null;
        }

        public override void OnEnter()
        {
            this.DoFlip();
            base.Finish();
        }

        private void DoFlip()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!(ownerDefaultTarget != null))
            {
                return;
            }
            RectTransform component = ownerDefaultTarget.GetComponent<RectTransform>();
            if (component != null)
            {
                if (this.axis == RectTransformFlipOptions.Both)
                {
                    RectTransformUtility.FlipLayoutAxes(component, this.keepPositioning.Value, this.recursive.Value);
                }
                else if (this.axis == RectTransformFlipOptions.Horizontal)
                {
                    RectTransformUtility.FlipLayoutOnAxis(component, 0, this.keepPositioning.Value, this.recursive.Value);
                }
                else if (this.axis == RectTransformFlipOptions.Vertical)
                {
                    RectTransformUtility.FlipLayoutOnAxis(component, 1, this.keepPositioning.Value, this.recursive.Value);
                }
            }
        }
    }
}
