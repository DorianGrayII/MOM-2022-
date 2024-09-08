namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [HutongGames.PlayMaker.Tooltip("Base class for actions that want to run a sub FSM.")]
    public abstract class RunFSMAction : FsmStateAction
    {
        protected Fsm runFsm;

        protected RunFSMAction()
        {
        }

        protected virtual void CheckIfFinished()
        {
            if ((this.runFsm == null) || this.runFsm.Finished)
            {
                base.Finish();
            }
        }

        public override void DoCollisionEnter(UnityEngine.Collision collisionInfo)
        {
            if (this.runFsm.HandleCollisionEnter)
            {
                this.runFsm.OnCollisionEnter(collisionInfo);
            }
        }

        public override void DoCollisionEnter2D(Collision2D collisionInfo)
        {
            if (this.runFsm.HandleCollisionEnter2D)
            {
                this.runFsm.OnCollisionEnter2D(collisionInfo);
            }
        }

        public override void DoCollisionExit(UnityEngine.Collision collisionInfo)
        {
            if (this.runFsm.HandleCollisionExit)
            {
                this.runFsm.OnCollisionExit(collisionInfo);
            }
        }

        public override void DoCollisionExit2D(Collision2D collisionInfo)
        {
            if (this.runFsm.HandleCollisionExit2D)
            {
                this.runFsm.OnCollisionExit2D(collisionInfo);
            }
        }

        public override void DoCollisionStay(UnityEngine.Collision collisionInfo)
        {
            if (this.runFsm.HandleCollisionStay)
            {
                this.runFsm.OnCollisionStay(collisionInfo);
            }
        }

        public override void DoCollisionStay2D(Collision2D collisionInfo)
        {
            if (this.runFsm.HandleCollisionStay2D)
            {
                this.runFsm.OnCollisionStay2D(collisionInfo);
            }
        }

        public override void DoControllerColliderHit(ControllerColliderHit collisionInfo)
        {
            if (this.runFsm.HandleControllerColliderHit)
            {
                this.runFsm.OnControllerColliderHit(collisionInfo);
            }
        }

        public override void DoParticleCollision(GameObject other)
        {
            if (this.runFsm.HandleParticleCollision)
            {
                this.runFsm.OnParticleCollision(other);
            }
        }

        public override void DoTriggerEnter(Collider other)
        {
            if (this.runFsm.HandleTriggerEnter)
            {
                this.runFsm.OnTriggerEnter(other);
            }
        }

        public override void DoTriggerEnter2D(Collider2D other)
        {
            if (this.runFsm.HandleTriggerEnter2D)
            {
                this.runFsm.OnTriggerEnter2D(other);
            }
        }

        public override void DoTriggerExit(Collider other)
        {
            if (this.runFsm.HandleTriggerExit)
            {
                this.runFsm.OnTriggerExit(other);
            }
        }

        public override void DoTriggerExit2D(Collider2D other)
        {
            if (this.runFsm.HandleTriggerExit2D)
            {
                this.runFsm.OnTriggerExit2D(other);
            }
        }

        public override void DoTriggerStay(Collider other)
        {
            if (this.runFsm.HandleTriggerStay)
            {
                this.runFsm.OnTriggerStay(other);
            }
        }

        public override void DoTriggerStay2D(Collider2D other)
        {
            if (this.runFsm.HandleTriggerStay2D)
            {
                this.runFsm.OnTriggerStay2D(other);
            }
        }

        public override bool Event(FsmEvent fsmEvent)
        {
            if ((this.runFsm != null) && (fsmEvent.IsGlobal || fsmEvent.IsSystemEvent))
            {
                this.runFsm.Event(fsmEvent);
            }
            return false;
        }

        public override void OnEnter()
        {
            if (this.runFsm == null)
            {
                base.Finish();
            }
            else
            {
                this.runFsm.OnEnable();
                if (!this.runFsm.Started)
                {
                    this.runFsm.Start();
                }
                this.CheckIfFinished();
            }
        }

        public override void OnExit()
        {
            if (this.runFsm != null)
            {
                this.runFsm.Stop();
            }
        }

        public override void OnFixedUpdate()
        {
            if (this.runFsm == null)
            {
                base.Finish();
            }
            else
            {
                this.runFsm.FixedUpdate();
                this.CheckIfFinished();
            }
        }

        public override void OnGUI()
        {
            if ((this.runFsm != null) && this.runFsm.HandleOnGUI)
            {
                this.runFsm.OnGUI();
            }
        }

        public override void OnLateUpdate()
        {
            if (this.runFsm == null)
            {
                base.Finish();
            }
            else
            {
                this.runFsm.LateUpdate();
                this.CheckIfFinished();
            }
        }

        public override void OnUpdate()
        {
            if (this.runFsm == null)
            {
                base.Finish();
            }
            else
            {
                this.runFsm.Update();
                this.CheckIfFinished();
            }
        }

        public override void Reset()
        {
            this.runFsm = null;
        }
    }
}

