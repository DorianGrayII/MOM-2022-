﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using System.Collections;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Set the Tag on all children of a GameObject. Optionally filter by component.")]
    public class SetTagsOnChildren : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("GameObject Parent")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Tag), HutongGames.PlayMaker.Tooltip("Set Tag To...")]
        public FsmString tag;
        [UIHint(UIHint.ScriptComponent), HutongGames.PlayMaker.Tooltip("Only set the Tag on children with this component.")]
        public FsmString filterByComponent;
        private System.Type componentFilter;

        public override void OnEnter()
        {
            this.SetTag(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.tag = null;
            this.filterByComponent = null;
        }

        private void SetTag(GameObject parent)
        {
            if (parent != null)
            {
                if (!string.IsNullOrEmpty(this.filterByComponent.Value))
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
                else
                {
                    using (IEnumerator enumerator = parent.transform.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            ((Transform) enumerator.Current).gameObject.tag = this.tag.Value;
                        }
                    }
                }
                base.Finish();
            }
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

