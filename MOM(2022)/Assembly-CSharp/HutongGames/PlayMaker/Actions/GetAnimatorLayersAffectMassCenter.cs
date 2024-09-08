namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Returns if additional layers affects the mass center")]
    public class GetAnimatorLayersAffectMassCenter : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [ActionSection("Results"), RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("If true, additional layers affects the mass center")]
        public FsmBool affectMassCenter;
        [HutongGames.PlayMaker.Tooltip("Event send if additional layers affects the mass center")]
        public FsmEvent affectMassCenterEvent;
        [HutongGames.PlayMaker.Tooltip("Event send if additional layers do no affects the mass center")]
        public FsmEvent doNotAffectMassCenterEvent;
        private Animator _animator;

        private void CheckAffectMassCenter()
        {
            if (this._animator != null)
            {
                bool layersAffectMassCenter = this._animator.layersAffectMassCenter;
                this.affectMassCenter.Value = layersAffectMassCenter;
                if (layersAffectMassCenter)
                {
                    base.Fsm.Event(this.affectMassCenterEvent);
                }
                else
                {
                    base.Fsm.Event(this.doNotAffectMassCenterEvent);
                }
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                base.Finish();
            }
            else
            {
                this._animator = ownerDefaultTarget.GetComponent<Animator>();
                if (this._animator == null)
                {
                    base.Finish();
                }
                else
                {
                    this.CheckAffectMassCenter();
                    base.Finish();
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.affectMassCenter = null;
            this.affectMassCenterEvent = null;
            this.doNotAffectMassCenterEvent = null;
        }
    }
}

