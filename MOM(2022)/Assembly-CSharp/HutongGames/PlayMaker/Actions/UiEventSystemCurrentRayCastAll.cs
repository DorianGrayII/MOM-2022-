using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("The eventType will be executed on all components on the GameObject that can handle it.")]
    public class UiEventSystemCurrentRayCastAll : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The ScreenPosition in pixels")]
        public FsmVector3 screenPosition;

        [Tooltip("The ScreenPosition in a Vector2")]
        public FsmVector2 orScreenPosition2d;

        [Tooltip("GameObjects hit by the raycast")]
        [UIHint(UIHint.Variable)]
        [ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
        public FsmArray gameObjectList;

        [Tooltip("Number of hits")]
        [UIHint(UIHint.Variable)]
        public FsmInt hitCount;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private PointerEventData pointer;

        private List<RaycastResult> raycastResults = new List<RaycastResult>();

        public override void Reset()
        {
            this.screenPosition = null;
            this.orScreenPosition2d = new FsmVector2
            {
                UseVariable = true
            };
            this.gameObjectList = null;
            this.hitCount = null;
            this.everyFrame = false;
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

        private void ExecuteRayCastAll()
        {
            this.pointer = new PointerEventData(EventSystem.current);
            if (!this.orScreenPosition2d.IsNone)
            {
                this.pointer.position = this.orScreenPosition2d.Value;
            }
            else
            {
                this.pointer.position = new Vector2(this.screenPosition.Value.x, this.screenPosition.Value.y);
            }
            EventSystem.current.RaycastAll(this.pointer, this.raycastResults);
            if (!this.hitCount.IsNone)
            {
                this.hitCount.Value = this.raycastResults.Count;
            }
            this.gameObjectList.Resize(this.raycastResults.Count);
            int index = 0;
            foreach (RaycastResult raycastResult in this.raycastResults)
            {
                if (!this.gameObjectList.IsNone)
                {
                    this.gameObjectList.Set(index, raycastResult.gameObject);
                }
            }
        }
    }
}
