namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("The eventType will be executed on all components on the GameObject that can handle it.")]
    public class UiEventSystemCurrentRayCastAll : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The ScreenPosition in pixels")]
        public FsmVector3 screenPosition;
        [HutongGames.PlayMaker.Tooltip("The ScreenPosition in a Vector2")]
        public FsmVector2 orScreenPosition2d;
        [HutongGames.PlayMaker.Tooltip("GameObjects hit by the raycast"), UIHint(UIHint.Variable), ArrayEditor(VariableType.GameObject, "", 0, 0, 0x10000)]
        public FsmArray gameObjectList;
        [HutongGames.PlayMaker.Tooltip("Number of hits"), UIHint(UIHint.Variable)]
        public FsmInt hitCount;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;
        private PointerEventData pointer;
        private List<RaycastResult> raycastResults = new List<RaycastResult>();

        private void ExecuteRayCastAll()
        {
            this.pointer = new PointerEventData(EventSystem.current);
            this.pointer.position = this.orScreenPosition2d.IsNone ? new Vector2(this.screenPosition.get_Value().x, this.screenPosition.get_Value().y) : this.orScreenPosition2d.get_Value();
            EventSystem.current.RaycastAll(this.pointer, this.raycastResults);
            if (!this.hitCount.IsNone)
            {
                this.hitCount.Value = this.raycastResults.Count;
            }
            this.gameObjectList.Resize(this.raycastResults.Count);
            int index = 0;
            foreach (RaycastResult result in this.raycastResults)
            {
                if (!this.gameObjectList.IsNone)
                {
                    this.gameObjectList.Set(index, result.gameObject);
                }
            }
        }

        public override void OnEnter()
        {
            this.ExecuteRayCastAll();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.ExecuteRayCastAll();
        }

        public override void Reset()
        {
            this.screenPosition = null;
            FsmVector2 vector1 = new FsmVector2();
            vector1.UseVariable = true;
            this.orScreenPosition2d = vector1;
            this.gameObjectList = null;
            this.hitCount = null;
            this.everyFrame = false;
        }
    }
}

