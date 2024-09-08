using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Input)]
    [Tooltip("Sends Events based on mouse interactions with a 2d Game Object: MouseOver, MouseDown, MouseUp, MouseOff.")]
    public class MousePick2dEvent : FsmStateAction
    {
        [CheckForComponent(typeof(Collider2D))]
        [Tooltip("The GameObject with a Collider2D attached.")]
        public FsmOwnerDefault GameObject;

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
            if (this.DoRaycast())
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
            GameObject gameObject = ((this.GameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.GameObject.GameObject.Value);
            RaycastHit2D rayIntersection = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), float.PositiveInfinity, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
            Fsm.RecordLastRaycastHit2DInfo(base.Fsm, rayIntersection);
            if (rayIntersection.transform != null && rayIntersection.transform.gameObject == gameObject)
            {
                return true;
            }
            return false;
        }
    }
}
