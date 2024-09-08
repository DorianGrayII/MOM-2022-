using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Measures the Distance betweens 2 Game Objects and stores the result in a Float Variable.")]
    public class GetDistance : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Measure distance from this GameObject.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("Target GameObject.")]
        public FsmGameObject target;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the distance in a float variable.")]
        public FsmFloat storeResult;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = null;
            this.target = null;
            this.storeResult = null;
            this.everyFrame = true;
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

        private void DoGetDistance()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!(ownerDefaultTarget == null) && !(this.target.Value == null) && this.storeResult != null)
            {
                this.storeResult.Value = Vector3.Distance(ownerDefaultTarget.transform.position, this.target.Value.transform.position);
            }
        }
    }
}
