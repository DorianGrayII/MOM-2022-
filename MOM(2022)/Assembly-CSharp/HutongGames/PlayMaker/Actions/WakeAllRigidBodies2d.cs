using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Rigid bodies 2D start sleeping when they come to rest. This action wakes up all rigid bodies 2D in the scene. E.g., if you Set Gravity 2D and want objects at rest to respond.")]
    public class WakeAllRigidBodies2d : FsmStateAction
    {
        [Tooltip("Repeat every frame. Note: This would be very expensive!")]
        public bool everyFrame;

        public override void Reset()
        {
            this.everyFrame = false;
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

        private void DoWakeAll()
        {
            if (Object.FindObjectsOfType(typeof(Rigidbody2D)) is Rigidbody2D[] array)
            {
                Rigidbody2D[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    array2[i].WakeUp();
                }
            }
        }
    }
}
