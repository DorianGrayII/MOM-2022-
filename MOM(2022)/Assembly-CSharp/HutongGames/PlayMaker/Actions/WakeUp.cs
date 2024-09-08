using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics)]
    [Tooltip("Forces a Game Object's Rigid Body to wake up.")]
    public class WakeUp : ComponentAction<Rigidbody>
    {
        [RequiredField]
        [CheckForComponent(typeof(Rigidbody))]
        public FsmOwnerDefault gameObject;

        public override void Reset()
        {
            this.gameObject = null;
        }

        public override void OnEnter()
        {
            this.DoWakeUp();
            base.Finish();
        }

        private void DoWakeUp()
        {
            GameObject go = ((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
            if (base.UpdateCache(go))
            {
                base.rigidbody.WakeUp();
            }
        }
    }
}
