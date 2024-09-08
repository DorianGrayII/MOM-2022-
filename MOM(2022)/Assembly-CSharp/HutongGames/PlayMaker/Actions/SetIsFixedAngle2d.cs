namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [Obsolete("This action is obsolete; use Constraints instead."), ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Controls whether the rigidbody 2D should be prevented from rotating")]
    public class SetIsFixedAngle2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField, CheckForComponent(typeof(Rigidbody2D)), HutongGames.PlayMaker.Tooltip("The GameObject with the Rigidbody2D attached")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The flag value")]
        public FsmBool isFixedAngle;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoSetIsFixedAngle()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                if (this.isFixedAngle.Value)
                {
                    base.rigidbody2d.constraints |= RigidbodyConstraints2D.FreezeRotation;
                }
                else
                {
                    base.rigidbody2d.constraints &= ~RigidbodyConstraints2D.FreezeRotation;
                }
            }
        }

        public override void OnEnter()
        {
            this.DoSetIsFixedAngle();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetIsFixedAngle();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.isFixedAngle = false;
            this.everyFrame = false;
        }
    }
}

