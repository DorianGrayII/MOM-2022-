using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Set the Tag on all children of a GameObject. Optionally filter by component.")]
    public class SetTagsOnChildren : FsmStateAction
    {
        [RequiredField]
        [Tooltip("GameObject Parent")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Tag)]
        [Tooltip("Set Tag To...")]
        public FsmString tag;

        [UIHint(UIHint.ScriptComponent)]
        [Tooltip("Only set the Tag on children with this component.")]
        public FsmString filterByComponent;

        private Type componentFilter;

        public override void Reset()
        {
            this.gameObject = null;
            this.tag = null;
            this.filterByComponent = null;
        }

        public override void OnEnter()
        {
            this.SetTag(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
            base.Finish();
        }

        private void SetTag(GameObject parent)
        {
            if (parent == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(this.filterByComponent.Value))
            {
                foreach (Transform item in parent.transform)
                {
                    item.gameObject.tag = this.tag.Value;
                }
            }
            else
            {
                this.UpdateComponentFilter();
                if (this.componentFilter != null)
                {
                    Component[] componentsInChildren = parent.GetComponentsInChildren(this.componentFilter);
                    for (int i = 0; i < componentsInChildren.Length; i++)
                    {
                        componentsInChildren[i].gameObject.tag = this.tag.Value;
                    }
                }
            }
            base.Finish();
        }

        private void UpdateComponentFilter()
        {
            this.componentFilter = ReflectionUtils.GetGlobalType(this.filterByComponent.Value);
            if (this.componentFilter == null)
            {
                this.componentFilter = ReflectionUtils.GetGlobalType("UnityEngine." + this.filterByComponent.Value);
            }
            if (this.componentFilter == null)
            {
                Debug.LogWarning("Couldn't get type: " + this.filterByComponent.Value);
            }
        }
    }
}
