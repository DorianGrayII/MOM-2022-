﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.AI;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Synchronize a NavMesh Agent velocity and rotation with the animator process.")]
    public class NavMeshAgentAnimatorSynchronizer : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(NavMeshAgent)), CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The Agent target. An Animator component and a NavMeshAgent component are required")]
        public FsmOwnerDefault gameObject;
        private Animator _animator;
        private NavMeshAgent _agent;
        private Transform _trans;

        public override void DoAnimatorMove()
        {
            this._agent.velocity = this._animator.deltaPosition / Time.deltaTime;
            this._trans.rotation = this._animator.rootRotation;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                base.Finish();
            }
            else
            {
                this._agent = ownerDefaultTarget.GetComponent<NavMeshAgent>();
                this._animator = ownerDefaultTarget.GetComponent<Animator>();
                if (this._animator == null)
                {
                    base.Finish();
                }
                else
                {
                    this._trans = ownerDefaultTarget.transform;
                }
            }
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleAnimatorMove = true;
        }

        public override void Reset()
        {
            this.gameObject = null;
        }
    }
}

