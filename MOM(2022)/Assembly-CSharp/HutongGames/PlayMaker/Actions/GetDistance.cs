namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Measures the Distance betweens 2 Game Objects and stores the result in a Float Variable.")]
    public class GetDistance : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("Measure distance from this GameObject.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Target GameObject.")]
        public FsmGameObject target;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the distance in a float variable.")]
        public FsmFloat storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoGetDistance()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((ownerDefaultTarget != null) && ((this.target.get_Value() != null) && (this.storeResult != null)))
            {
                this.storeResult.Value = Vector3.Distance(ownerDefaultTarget.transform.position, this.target.get_Value().transform.position);
            }
        }

        public override void OnEnter()
        {
            this.DoGetDistance();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetDistance();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.target = null;
            this.storeResult = null;
            this.everyFrame = true;
        }
    }
}

