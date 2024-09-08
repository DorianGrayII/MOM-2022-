using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Returns true if a parameter is controlled by an additional curve on an animation")]
    public class GetAnimatorIsParameterControlledByCurve : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The parameter's name")]
        public FsmString parameterName;

        [ActionSection("Results")]
        [UIHint(UIHint.Variable)]
        [Tooltip("True if controlled by curve")]
        public FsmBool isControlledByCurve;

        [Tooltip("Event send if controlled by curve")]
        public FsmEvent isControlledByCurveEvent;

        [Tooltip("Event send if not controlled by curve")]
        public FsmEvent isNotControlledByCurveEvent;

        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = null;
            this.parameterName = null;
            this.isControlledByCurve = null;
            this.isControlledByCurveEvent = null;
            this.isNotControlledByCurveEvent = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                base.Finish();
                return;
            }
            this._animator = ownerDefaultTarget.GetComponent<Animator>();
            if (this._animator == null)
            {
                base.Finish();
                return;
            }
            this.DoCheckIsParameterControlledByCurve();
            base.Finish();
        }

        private void DoCheckIsParameterControlledByCurve()
        {
            if (!(this._animator == null))
            {
                bool flag = this._animator.IsParameterControlledByCurve(this.parameterName.Value);
                this.isControlledByCurve.Value = flag;
                if (flag)
                {
                    base.Fsm.Event(this.isControlledByCurveEvent);
                }
                else
                {
                    base.Fsm.Event(this.isNotControlledByCurveEvent);
                }
            }
        }
    }
}
