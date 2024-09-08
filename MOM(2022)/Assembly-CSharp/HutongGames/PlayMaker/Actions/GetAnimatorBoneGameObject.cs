namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Gets the GameObject mapped to this human bone id")]
    public class GetAnimatorBoneGameObject : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The bone reference"), ObjectType(typeof(HumanBodyBones))]
        public FsmEnum bone;
        [ActionSection("Results"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The Bone's GameObject")]
        public FsmGameObject boneGameObject;
        private Animator _animator;

        private void GetBoneTransform()
        {
            this.boneGameObject.set_Value(this._animator.GetBoneTransform((HumanBodyBones) this.bone.Value).gameObject);
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
                    this.GetBoneTransform();
                    base.Finish();
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.bone = HumanBodyBones.Hips;
            this.boneGameObject = null;
        }
    }
}

