using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Gets the GameObject mapped to this human bone id")]
    public class GetAnimatorBoneGameObject : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The bone reference")]
        [ObjectType(typeof(HumanBodyBones))]
        public FsmEnum bone;

        [ActionSection("Results")]
        [UIHint(UIHint.Variable)]
        [Tooltip("The Bone's GameObject")]
        public FsmGameObject boneGameObject;

        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = null;
            this.bone = HumanBodyBones.Hips;
            this.boneGameObject = null;
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
            this.GetBoneTransform();
            base.Finish();
        }

        private void GetBoneTransform()
        {
            this.boneGameObject.Value = this._animator.GetBoneTransform((HumanBodyBones)(object)this.bone.Value).gameObject;
        }
    }
}
