﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Rigid bodies start sleeping when they come to rest. This action wakes up all rigid bodies in the scene. E.g., if you Set Gravity and want objects at rest to respond.")]
    public class WakeAllRigidBodies : FsmStateAction
    {
        public bool everyFrame;
        private Rigidbody[] bodies;

        private void DoWakeAll()
        {
            this.bodies = UnityEngine.Object.FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[];
            if (this.bodies != null)
            {
                Rigidbody[] bodies = this.bodies;
                for (int i = 0; i < bodies.Length; i++)
                {
                    bodies[i].WakeUp();
                }
            }
        }

        public override void OnEnter()
        {
            this.bodies = UnityEngine.Object.FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[];
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

