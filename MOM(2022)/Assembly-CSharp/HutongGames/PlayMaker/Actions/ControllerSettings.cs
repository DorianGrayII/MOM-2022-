namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Character), HutongGames.PlayMaker.Tooltip("Modify various character controller settings.\n'None' leaves the setting unchanged.")]
    public class ControllerSettings : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(CharacterController)), HutongGames.PlayMaker.Tooltip("The GameObject that owns the CharacterController.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The height of the character's capsule.")]
        public FsmFloat height;
        [HutongGames.PlayMaker.Tooltip("The radius of the character's capsule.")]
        public FsmFloat radius;
        [HutongGames.PlayMaker.Tooltip("The character controllers slope limit in degrees.")]
        public FsmFloat slopeLimit;
        [HutongGames.PlayMaker.Tooltip("The character controllers step offset in meters.")]
        public FsmFloat stepOffset;
        [HutongGames.PlayMaker.Tooltip("The center of the character's capsule relative to the transform's position")]
        public FsmVector3 center;
        [HutongGames.PlayMaker.Tooltip("Should other rigidbodies or character controllers collide with this character controller (By default always enabled).")]
        public FsmBool detectCollisions;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;
        private GameObject previousGo;
        private CharacterController controller;

        private void DoControllerSettings()
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
                    if (!this.height.IsNone)
                    {
                        this.controller.height = this.height.Value;
                    }
                    if (!this.radius.IsNone)
                    {
                        this.controller.radius = this.radius.Value;
                    }
                    if (!this.slopeLimit.IsNone)
                    {
                        this.controller.slopeLimit = this.slopeLimit.Value;
                    }
                    if (!this.stepOffset.IsNone)
                    {
                        this.controller.stepOffset = this.stepOffset.Value;
                    }
                    if (!this.center.IsNone)
                    {
                        this.controller.center = this.center.get_Value();
                    }
                    if (!this.detectCollisions.IsNone)
                    {
                        this.controller.detectCollisions = this.detectCollisions.Value;
                    }
                }
            }
        }

        public override void OnEnter()
        {
            this.DoControllerSettings();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoControllerSettings();
        }

        public override void Reset()
        {
            this.gameObject = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.height = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.radius = num2;
            FsmFloat num3 = new FsmFloat();
            num3.UseVariable = true;
            this.slopeLimit = num3;
            FsmFloat num4 = new FsmFloat();
            num4.UseVariable = true;
            this.stepOffset = num4;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.center = vector1;
            FsmBool bool1 = new FsmBool();
            bool1.UseVariable = true;
            this.detectCollisions = bool1;
            this.everyFrame = false;
        }
    }
}

