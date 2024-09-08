namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("Flips the horizontal and vertical axes of the RectTransform size and alignment, and optionally its children as well.")]
    public class RectTransformFlipLayoutAxis : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The axis to flip")]
        public RectTransformFlipOptions axis;
        [HutongGames.PlayMaker.Tooltip("Flips around the pivot if true. Flips within the parent rect if false.")]
        public FsmBool keepPositioning;
        [HutongGames.PlayMaker.Tooltip("Flip the children as well?")]
        public FsmBool recursive;

        private void DoFlip()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
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

        public override void OnEnter()
        {
            this.DoFlip();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.axis = RectTransformFlipOptions.Both;
            this.keepPositioning = null;
            this.recursive = null;
        }

        public enum RectTransformFlipOptions
        {
            Horizontal,
            Vertical,
            Both
        }
    }
}

