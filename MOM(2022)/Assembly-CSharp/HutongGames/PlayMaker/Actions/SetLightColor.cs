namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Lights), HutongGames.PlayMaker.Tooltip("Sets the Color of a Light.")]
    public class SetLightColor : ComponentAction<Light>
    {
        [RequiredField, CheckForComponent(typeof(Light))]
        public FsmOwnerDefault gameObject;
        [RequiredField]
        public FsmColor lightColor;
        public bool everyFrame;

        private void DoSetLightColor()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                base.light.color = this.lightColor.get_Value();
            }
        }

        public override void OnEnter()
        {
            this.DoSetLightColor();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetLightColor();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.lightColor = (FsmColor) Color.white;
            this.everyFrame = false;
        }
    }
}

