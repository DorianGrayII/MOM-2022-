namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Rigid bodies 2D start sleeping when they come to rest. This action wakes up all rigid bodies 2D in the scene. E.g., if you Set Gravity 2D and want objects at rest to respond.")]
    public class WakeAllRigidBodies2d : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Note: This would be very expensive!")]
        public bool everyFrame;

        private void DoWakeAll()
        {
            Rigidbody2D[] rigidbodydArray = UnityEngine.Object.FindObjectsOfType(typeof(Rigidbody2D)) as Rigidbody2D[];
            if (rigidbodydArray != null)
            {
                Rigidbody2D[] rigidbodydArray2 = rigidbodydArray;
                for (int i = 0; i < rigidbodydArray2.Length; i++)
                {
                    rigidbodydArray2[i].WakeUp();
                }
            }
        }

        public override void OnEnter()
        {
            this.DoWakeAll();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoWakeAll();
        }

        public override void Reset()
        {
            this.everyFrame = false;
        }
    }
}

