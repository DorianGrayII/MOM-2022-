namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Input), ActionTarget(typeof(UnityEngine.GameObject), "GameObject", false), HutongGames.PlayMaker.Tooltip("Sends Events based on mouse interactions with a Game Object: MouseOver, MouseDown, MouseUp, MouseOff. Use Ray Distance to set how close the camera must be to pick the object.\n\nNOTE: Picking uses the Main Camera.")]
    public class MousePickEvent : FsmStateAction
    {
        [CheckForComponent(typeof(Collider))]
        public FsmOwnerDefault GameObject;
        [HutongGames.PlayMaker.Tooltip("Length of the ray to cast from the camera.")]
        public FsmFloat rayDistance = 100f;
        [HutongGames.PlayMaker.Tooltip("Event to send when the mouse is over the GameObject.")]
        public FsmEvent mouseOver;
        [HutongGames.PlayMaker.Tooltip("Event to send when the mouse is pressed while over the GameObject.")]
        public FsmEvent mouseDown;
        [HutongGames.PlayMaker.Tooltip("Event to send when the mouse is released while over the GameObject.")]
        public FsmEvent mouseUp;
        [HutongGames.PlayMaker.Tooltip("Event to send when the mouse moves off the GameObject.")]
        public FsmEvent mouseOff;
        [HutongGames.PlayMaker.Tooltip("Pick only from these layers."), UIHint(UIHint.Layer)]
        public FsmInt[] layerMask;
        [HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
        public FsmBool invertMask;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoMousePickEvent()
        {
            base.Fsm.set_RaycastHitInfo(ActionHelpers.mousePickInfo);
            if (!this.DoRaycast())
            {
                if (this.mouseOff != null)
                {
                    base.Fsm.Event(this.mouseOff);
                }
            }
            else
            {
                if ((this.mouseDown != null) && Input.GetMouseButtonDown(0))
                {
                    base.Fsm.Event(this.mouseDown);
                }
                if (this.mouseOver != null)
                {
                    base.Fsm.Event(this.mouseOver);
                }
                if ((this.mouseUp != null) && Input.GetMouseButtonUp(0))
                {
                    base.Fsm.Event(this.mouseUp);
                }
            }
        }

        private bool DoRaycast()
        {
            return ActionHelpers.IsMouseOver((this.GameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.get_Owner() : this.GameObject.GameObject.get_Value(), this.rayDistance.Value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
        }

        public override string ErrorCheck()
        {
            return ("" + ActionHelpers.CheckRayDistance(this.rayDistance.Value) + ActionHelpers.CheckPhysicsSetup(base.Fsm.GetOwnerDefaultTarget(this.GameObject)));
        }

        public override void OnEnter()
        {
            this.DoMousePickEvent();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoMousePickEvent();
        }

        public override void Reset()
        {
            this.GameObject = null;
            this.rayDistance = 100f;
            this.mouseOver = null;
            this.mouseDown = null;
            this.mouseUp = null;
            this.mouseOff = null;
            this.layerMask = new FsmInt[0];
            this.invertMask = false;
            this.everyFrame = true;
        }
    }
}

