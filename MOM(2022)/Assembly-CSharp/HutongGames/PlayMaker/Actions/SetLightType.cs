using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Lights)]
    [Tooltip("Set Spot, Directional, or Point Light type.")]
    public class SetLightType : ComponentAction<Light>
    {
        [RequiredField]
        [CheckForComponent(typeof(Light))]
        public FsmOwnerDefault gameObject;

        [ObjectType(typeof(LightType))]
        public FsmEnum lightType;

        public override void Reset()
        {
            this.gameObject = null;
            this.lightType = LightType.Point;
        }

        public override void OnEnter()
        {
            this.DoSetLightType();
            base.Finish();
        }

        private void DoSetLightType()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                base.light.type = (LightType)(object)this.lightType.Value;
            }
        }
    }
}
