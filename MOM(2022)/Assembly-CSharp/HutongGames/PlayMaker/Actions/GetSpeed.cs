namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Gets the Speed of a Game Object and stores it in a Float Variable. NOTE: The Game Object must have a rigid body.")]
    public class GetSpeed : ComponentAction<Rigidbody>
    {
        [RequiredField, CheckForComponent(typeof(Rigidbody)), HutongGames.PlayMaker.Tooltip("The GameObject with a Rigidbody.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the speed in a float variable.")]
        public FsmFloat storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoGetSpeed()
        {
            if (this.storeResult != null)
            {
                GameObject go = (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.get_Owner() : this.gameObject.GameObject.get_Value();
                if (base.UpdateCache(go))
                {
                    Vector3 velocity = base.rigidbody.velocity;
                    this.storeResult.Value = velocity.magnitude;
                }
            }
        }

        public override void OnEnter()
        {
            this.DoGetSpeed();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetSpeed();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.storeResult = null;
            this.everyFrame = false;
        }
    }
}

