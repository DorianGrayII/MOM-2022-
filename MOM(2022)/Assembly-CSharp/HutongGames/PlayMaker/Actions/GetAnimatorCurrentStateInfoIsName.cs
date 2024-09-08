using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Check the current State name on a specified layer, this is more than the layer name, it holds the current state as well.")]
    public class GetAnimatorCurrentStateInfoIsName : FsmStateActionAnimatorBase
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The layer's index")]
        public FsmInt layerIndex;

        [Tooltip("The name to check the layer against.")]
        public FsmString name;

        [ActionSection("Results")]
        [UIHint(UIHint.Variable)]
        [Tooltip("True if name matches")]
        public FsmBool isMatching;

        [Tooltip("Event send if name matches")]
        public FsmEvent nameMatchEvent;

        [Tooltip("Event send if name doesn't match")]
        public FsmEvent nameDoNotMatchEvent;

        private Animator _animator;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.layerIndex = null;
            this.name = null;
            this.nameMatchEvent = null;
            this.nameDoNotMatchEvent = null;
            base.everyFrame = false;
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
            this.IsName();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.IsName();
        }

        private void IsName()
        {
            if (this._animator != null)
            {
                AnimatorStateInfo currentAnimatorStateInfo = this._animator.GetCurrentAnimatorStateInfo(this.layerIndex.Value);
                if (!this.isMatching.IsNone)
                {
                    this.isMatching.Value = currentAnimatorStateInfo.IsName(this.name.Value);
                }
                if (currentAnimatorStateInfo.IsName(this.name.Value))
                {
                    base.Fsm.Event(this.nameMatchEvent);
                }
                else
                {
                    base.Fsm.Event(this.nameDoNotMatchEvent);
                }
            }
        }
    }
}
