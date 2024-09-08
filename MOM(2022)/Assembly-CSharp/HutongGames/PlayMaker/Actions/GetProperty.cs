namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.UnityObject), ActionTarget(typeof(Component), "targetProperty", false), ActionTarget(typeof(GameObject), "targetProperty", false), HutongGames.PlayMaker.Tooltip("Gets the value of any public property or field on the targeted Unity Object and stores it in a variable. E.g., Drag and drop any component attached to a Game Object to access its properties.")]
    public class GetProperty : FsmStateAction
    {
        public FsmProperty targetProperty;
        public bool everyFrame;

        public override void OnEnter()
        {
            this.targetProperty.GetValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.targetProperty.GetValue();
        }

        public override void Reset()
        {
            FsmProperty property1 = new FsmProperty();
            property1.setProperty = false;
            this.targetProperty = property1;
            this.everyFrame = false;
        }
    }
}

