using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Sets the Mass of a Game Object's Rigid Body 2D.")]
    public class SetMass2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField]
        [CheckForComponent(typeof(Rigidbody2D))]
        [Tooltip("The GameObject with the Rigidbody2D attached")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [HasFloatSlider(0.1f, 10f)]
        [Tooltip("The Mass")]
        public FsmFloat mass;

        public override void Reset()
        {
            this.gameObject = null;
            this.mass = 1f;
        }

        public override void OnEnter()
        {
            this.DoSetMass();
            base.Finish();
        }

        private void DoSetMass()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                base.rigidbody2d.mass = this.mass.Value;
            }
        }
    }
}
