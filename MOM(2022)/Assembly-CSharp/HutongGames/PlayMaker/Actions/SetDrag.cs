using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics)]
    [HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=4734.0")]
    [Tooltip("Sets the Drag of a Game Object's Rigid Body.")]
    public class SetDrag : ComponentAction<Rigidbody>
    {
        [RequiredField]
        [CheckForComponent(typeof(Rigidbody))]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [HasFloatSlider(0f, 10f)]
        public FsmFloat drag;

        [Tooltip("Repeat every frame. Typically this would be set to True.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = null;
            this.drag = 1f;
        }

        public override void OnEnter()
        {
            this.DoSetDrag();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetDrag();
        }

        private void DoSetDrag()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                base.rigidbody.drag = this.drag.Value;
            }
        }
    }
}
