namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Lights), HutongGames.PlayMaker.Tooltip("Set Spot, Directional, or Point Light type.")]
    public class SetLightType : ComponentAction<Light>
    {
        [RequiredField, CheckForComponent(typeof(Light))]
        public FsmOwnerDefault gameObject;
        [ObjectType(typeof(LightType))]
        public FsmEnum lightType;

        private void DoSetLightType()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                base.light.type = (LightType) this.lightType.Value;
            }
        }

        public override void OnEnter()
        {
            this.DoSetLightType();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.lightType = LightType.Point;
        }
    }
}

