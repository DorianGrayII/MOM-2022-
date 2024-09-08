using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Base class for actions that want to run a sub FSM.")]
    public abstract class RunFSMAction : FsmStateAction
    {
        protected Fsm runFsm;

        public override void Reset()
        {
            this.runFsm = null;
        }

        public override bool Event(FsmEvent fsmEvent)
        {
            if (this.runFsm != null && (fsmEvent.IsGlobal || fsmEvent.IsSystemEvent))
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
                return;
            }
            this.runFsm.OnEnable();
            if (!this.runFsm.Started)
            {
                this.runFsm.Start();
            }
            this.CheckIfFinished();
        }

        public override void OnUpdate()
        {
            if (this.runFsm != null)
            {
                this.runFsm.Update();
                this.CheckIfFinished();
            }
            else
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            if (this.runFsm != null)
            {
                this.runFsm.FixedUpdate();
                this.CheckIfFinished();
            }
            else
            {
                base.Finish();
            }
        }

        public override void OnLateUpdate()
        {
            if (this.runFsm != null)
            {
                this.runFsm.LateUpdate();
                this.CheckIfFinished();
            }
            else
            {
                base.Finish();
            }
        }

        public override void DoTriggerEnter(Collider other)
        {
            if (this.runFsm.HandleTriggerEnter)
            {
                this.runFsm.OnTriggerEnter(other);
            }
        }

        public override void DoTriggerStay(Collider other)
        {
            if (this.runFsm.HandleTriggerStay)
            {
                this.runFsm.OnTriggerStay(other);
            }
        }

        public override void DoTriggerExit(Collider other)
        {
            if (this.runFsm.HandleTriggerExit)
            {
                this.runFsm.OnTriggerExit(other);
            }
        }

        public override void DoCollisionEnter(Collision collisionInfo)
        {
            if (this.runFsm.HandleCollisionEnter)
            {
                this.runFsm.OnCollisionEnter(collisionInfo);
            }
        }

        public override void DoCollisionStay(Collision collisionInfo)
        {
            if (this.runFsm.HandleCollisionStay)
            {
                this.runFsm.OnCollisionStay(collisionInfo);
            }
        }

        public override void DoCollisionExit(Collision collisionInfo)
        {
            if (this.runFsm.HandleCollisionExit)
            {
                this.runFsm.OnCollisionExit(collisionInfo);
            }
        }

        public override void DoParticleCollision(GameObject other)
        {
            if (this.runFsm.HandleParticleCollision)
            {
                this.runFsm.OnParticleCollision(other);
            }
        }

        public override void DoControllerColliderHit(ControllerColliderHit collisionInfo)
        {
            if (this.runFsm.HandleControllerColliderHit)
            {
                this.runFsm.OnControllerColliderHit(collisionInfo);
            }
        }

        public override void DoTriggerEnter2D(Collider2D other)
        {
            if (this.runFsm.HandleTriggerEnter2D)
            {
                this.runFsm.OnTriggerEnter2D(other);
            }
        }

        public override void DoTriggerStay2D(Collider2D other)
        {
            if (this.runFsm.HandleTriggerStay2D)
            {
                this.runFsm.OnTriggerStay2D(other);
            }
        }

        public override void DoTriggerExit2D(Collider2D other)
        {
            if (this.runFsm.HandleTriggerExit2D)
            {
                this.runFsm.OnTriggerExit2D(other);
            }
        }

        public override void DoCollisionEnter2D(Collision2D collisionInfo)
        {
            if (this.runFsm.HandleCollisionEnter2D)
            {
                this.runFsm.OnCollisionEnter2D(collisionInfo);
            }
        }

        public override void DoCollisionStay2D(Collision2D collisionInfo)
        {
            if (this.runFsm.HandleCollisionStay2D)
            {
                this.runFsm.OnCollisionStay2D(collisionInfo);
            }
        }

        public override void DoCollisionExit2D(Collision2D collisionInfo)
        {
            if (this.runFsm.HandleCollisionExit2D)
            {
                this.runFsm.OnCollisionExit2D(collisionInfo);
            }
        }

        public override void OnGUI()
        {
            if (this.runFsm != null && this.runFsm.HandleOnGUI)
            {
                this.runFsm.OnGUI();
            }
        }

        public override void OnExit()
        {
            if (this.runFsm != null)
            {
                this.runFsm.Stop();
            }
        }

        protected virtual void CheckIfFinished()
        {
            if (this.runFsm == null || this.runFsm.Finished)
            {
                base.Finish();
            }
        }
    }
}
