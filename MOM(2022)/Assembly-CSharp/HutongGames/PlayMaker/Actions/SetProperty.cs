namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.UnityObject), ActionTarget(typeof(Component), "targetProperty", false), ActionTarget(typeof(GameObject), "targetProperty", false), HutongGames.PlayMaker.Tooltip("Sets the value of any public property or field on the targeted Unity Object. E.g., Drag and drop any component attached to a Game Object to access its properties.")]
    public class SetProperty : FsmStateAction
    {
        public FsmProperty targetProperty;
        public bool everyFrame;

        public override void OnEnter()
        {
            this.targetProperty.SetValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.targetProperty.SetValue();
        }

        public override void Reset()
        {
            FsmProperty property1 = new FsmProperty();
            property1.setProperty = true;
            this.targetProperty = property1;
            this.everyFrame = false;
        }
    }
}

