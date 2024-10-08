using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Input)]
    [ActionTarget(typeof(GameObject), "GameObject", false)]
    [Tooltip("Sends Events based on mouse interactions with a Game Object: MouseOver, MouseDown, MouseUp, MouseOff. Use Ray Distance to set how close the camera must be to pick the object.\n\nNOTE: Picking uses the Main Camera.")]
    public class MousePickEvent : FsmStateAction
    {
        [CheckForComponent(typeof(Collider))]
        public FsmOwnerDefault GameObject;

        [Tooltip("Length of the ray to cast from the camera.")]
        public FsmFloat rayDistance = 100f;

        [Tooltip("Event to send when the mouse is over the GameObject.")]
        public FsmEvent mouseOver;

        [Tooltip("Event to send when the mouse is pressed while over the GameObject.")]
        public FsmEvent mouseDown;

        [Tooltip("Event to send when the mouse is released while over the GameObject.")]
        public FsmEvent mouseUp;

        [Tooltip("Event to send when the mouse moves off the GameObject.")]
        public FsmEvent mouseOff;

        [Tooltip("Pick only from these layers.")]
        [UIHint(UIHint.Layer)]
        public FsmInt[] layerMask;

        [Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
        public FsmBool invertMask;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

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

        private void DoMousePickEvent()
        {
            bool num = this.DoRaycast();
            base.Fsm.RaycastHitInfo = ActionHelpers.mousePickInfo;
            if (num)
            {
                if (this.mouseDown != null && Input.GetMouseButtonDown(0))
                {
                    base.Fsm.Event(this.mouseDown);
                }
                if (this.mouseOver != null)
                {
                    base.Fsm.Event(this.mouseOver);
                }
                if (this.mouseUp != null && Input.GetMouseButtonUp(0))
                {
                    base.Fsm.Event(this.mouseUp);
                }
            }
            else if (this.mouseOff != null)
            {
                base.Fsm.Event(this.mouseOff);
            }
        }

        private bool DoRaycast()
        {
            return ActionHelpers.IsMouseOver((this.GameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.GameObject.GameObject.Value, this.rayDistance.Value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
        }

        public override string ErrorCheck()
        {
            return string.Concat("" + ActionHelpers.CheckRayDistance(this.rayDistance.Value), ActionHelpers.CheckPhysicsSetup(base.Fsm.GetOwnerDefaultTarget(this.GameObject)));
        }
    }
}
