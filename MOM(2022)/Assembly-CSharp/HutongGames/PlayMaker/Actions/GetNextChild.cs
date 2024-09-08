﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Each time this action is called it gets the next child of a GameObject. This lets you quickly loop through all the children of an object to perform actions on them. NOTE: To find a specific child use Find Child.")]
    public class GetNextChild : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The parent GameObject. Note, if GameObject changes, this action will reset and start again at the first child.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the next child in a GameObject variable.")]
        public FsmGameObject storeNextChild;
        [HutongGames.PlayMaker.Tooltip("Event to send to get the next child.")]
        public FsmEvent loopEvent;
        [HutongGames.PlayMaker.Tooltip("Event to send when there are no more children.")]
        public FsmEvent finishedEvent;
        [HutongGames.PlayMaker.Tooltip("If you want to reset the iteration, raise this flag to true when you enter the state, it will indicate you want to start from the beginning again"), UIHint(UIHint.Variable)]
        public FsmBool resetFlag;
        private GameObject go;
        private int nextChildIndex;

        private void DoGetNextChild(GameObject parent)
        {
            if (parent != null)
            {
                if (this.go != parent)
                {
                    this.go = parent;
                    this.nextChildIndex = 0;
                }
                if (this.nextChildIndex >= this.go.transform.childCount)
                {
                    this.nextChildIndex = 0;
                    base.Fsm.Event(this.finishedEvent);
                }
                else
                {
                    this.storeNextChild.set_Value(parent.transform.GetChild(this.nextChildIndex).gameObject);
                    if (this.nextChildIndex >= this.go.transform.childCount)
                    {
                        this.nextChildIndex = 0;
                        base.Fsm.Event(this.finishedEvent);
                    }
                    else
                    {
                        this.nextChildIndex++;
                        if (this.loopEvent != null)
                        {
                            base.Fsm.Event(this.loopEvent);
                        }
                    }
                }
            }
        }

        public override void OnEnter()
        {
            if (this.resetFlag.Value)
            {
                this.nextChildIndex = 0;
                this.resetFlag.Value = false;
            }
            this.DoGetNextChild(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.storeNextChild = null;
            this.loopEvent = null;
            this.finishedEvent = null;
            this.resetFlag = null;
        }
    }
}

