namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using System.Collections.Generic;

    [NoActionTargets, ActionCategory(ActionCategory.Array), Tooltip("Transfer a value from one array to another, basically a copy/cut paste action on steroids.")]
    public class ArrayTransferValue : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The Array Variable source.")]
        public FsmArray arraySource;
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The Array Variable target.")]
        public FsmArray arrayTarget;
        [MatchFieldType("array"), Tooltip("The index to transfer.")]
        public FsmInt indexToTransfer;
        [ActionSection("Transfer Options"), ObjectType(typeof(ArrayTransferType))]
        public FsmEnum copyType;
        [ObjectType(typeof(ArrayPasteType))]
        public FsmEnum pasteType;
        [ActionSection("Result"), Tooltip("Event sent if this array source does not contains that element (described below)")]
        public FsmEvent indexOutOfRange;

        private void DoTransferValue()
        {
            if (!this.arraySource.IsNone && !this.arrayTarget.IsNone)
            {
                int index = this.indexToTransfer.Value;
                if ((index < 0) || (index >= this.arraySource.Length))
                {
                    base.Fsm.Event(this.indexOutOfRange);
                }
                else
                {
                    object item = this.arraySource.Values[index];
                    if (((ArrayTransferType) this.copyType.Value) == ArrayTransferType.Cut)
                    {
                        List<object> list = new List<object>(this.arraySource.Values);
                        list.RemoveAt(index);
                        this.arraySource.Values = list.ToArray();
                    }
                    else if (((ArrayTransferType) this.copyType.Value) == ArrayTransferType.nullify)
                    {
                        this.arraySource.Values.SetValue(null, index);
                    }
                    if (((ArrayPasteType) this.pasteType.Value) == ArrayPasteType.AsFirstItem)
                    {
                        List<object> list2 = new List<object>(this.arrayTarget.Values);
                        list2.Insert(0, item);
                        this.arrayTarget.Values = list2.ToArray();
                    }
                    else if (((ArrayPasteType) this.pasteType.Value) == ArrayPasteType.AsLastItem)
                    {
                        this.arrayTarget.Resize(this.arrayTarget.Length + 1);
                        this.arrayTarget.Set(this.arrayTarget.Length - 1, item);
                    }
                    else if (((ArrayPasteType) this.pasteType.Value) == ArrayPasteType.InsertAtSameIndex)
                    {
                        if (index >= this.arrayTarget.Length)
                        {
                            base.Fsm.Event(this.indexOutOfRange);
                        }
                        List<object> list3 = new List<object>(this.arrayTarget.Values);
                        list3.Insert(index, item);
                        this.arrayTarget.Values = list3.ToArray();
                    }
                    else if (((ArrayPasteType) this.pasteType.Value) == ArrayPasteType.ReplaceAtSameIndex)
                    {
                        if (index >= this.arrayTarget.Length)
                        {
                            base.Fsm.Event(this.indexOutOfRange);
                        }
                        else
                        {
                            this.arrayTarget.Set(index, item);
                        }
                    }
                }
            }
        }

        public override void OnEnter()
        {
            this.DoTransferValue();
            base.Finish();
        }

        public override void Reset()
        {
            this.arraySource = null;
            this.arrayTarget = null;
            this.indexToTransfer = null;
            this.copyType = ArrayTransferType.Copy;
            this.pasteType = ArrayPasteType.AsLastItem;
        }

        public enum ArrayPasteType
        {
            AsFirstItem,
            AsLastItem,
            InsertAtSameIndex,
            ReplaceAtSameIndex
        }

        public enum ArrayTransferType
        {
            Copy,
            Cut,
            nullify
        }
    }
}

