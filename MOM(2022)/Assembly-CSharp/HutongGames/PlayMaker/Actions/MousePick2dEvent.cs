namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Sends Events based on mouse interactions with a 2d Game Object: MouseOver, MouseDown, MouseUp, MouseOff.")]
    public class MousePick2dEvent : FsmStateAction
    {
        [CheckForComponent(typeof(Collider2D)), HutongGames.PlayMaker.Tooltip("The GameObject with a Collider2D attached.")]
        public FsmOwnerDefault GameObject;
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
            UnityEngine.GameObject obj2 = (this.GameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.get_Owner() : this.GameObject.GameObject.get_Value();
            RaycastHit2D hitd = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), float.PositiveInfinity, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
            Fsm.RecordLastRaycastHit2DInfo(base.Fsm, hitd);
            return ((hitd.transform != null) && (hitd.transform.gameObject == obj2));
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

