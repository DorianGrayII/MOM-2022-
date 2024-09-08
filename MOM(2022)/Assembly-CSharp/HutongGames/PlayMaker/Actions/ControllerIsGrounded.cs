﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Character), HutongGames.PlayMaker.Tooltip("Tests if a Character Controller on a Game Object was touching the ground during the last move.")]
    public class ControllerIsGrounded : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(CharacterController)), HutongGames.PlayMaker.Tooltip("The GameObject to check.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Event to send if touching the ground.")]
        public FsmEvent trueEvent;
        [HutongGames.PlayMaker.Tooltip("Event to send if not touching the ground.")]
        public FsmEvent falseEvent;
        [HutongGames.PlayMaker.Tooltip("Store the result in a bool variable."), UIHint(UIHint.Variable)]
        public FsmBool storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;
        private GameObject previousGo;
        private CharacterController controller;

        private void DoControllerIsGrounded()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                if (ownerDefaultTarget != this.previousGo)
                {
                    this.controller = ownerDefaultTarget.GetComponent<CharacterController>();
                    this.previousGo = ownerDefaultTarget;
                }
                if (this.controller != null)
                {
                    bool isGrounded = this.controller.isGrounded;
                    this.storeResult.Value = isGrounded;
                    base.Fsm.Event(isGrounded ? this.trueEvent : this.falseEvent);
                }
            }
        }

        public override void OnEnter()
        {
            this.DoControllerIsGrounded();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoControllerIsGrounded();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.trueEvent = null;
            this.falseEvent = null;
            this.storeResult = null;
            this.everyFrame = false;
        }
    }
}

