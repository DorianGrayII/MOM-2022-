using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
    [NoActionTargets]
    [ActionCategory(ActionCategory.Array)]
    [Tooltip("Transfer a value from one array to another, basically a copy/cut paste action on steroids.")]
    public class ArrayTransferValue : FsmStateAction
    {
        public enum ArrayTransferType
        {
            Copy = 0,
            Cut = 1,
            nullify = 2
        }

        public enum ArrayPasteType
        {
            AsFirstItem = 0,
            AsLastItem = 1,
            InsertAtSameIndex = 2,
            ReplaceAtSameIndex = 3
        }

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The Array Variable source.")]
        public FsmArray arraySource;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The Array Variable target.")]
        public FsmArray arrayTarget;

        [MatchFieldType("array")]
        [Tooltip("The index to transfer.")]
        public FsmInt indexToTransfer;

        [ActionSection("Transfer Options")]
        [ObjectType(typeof(ArrayTransferType))]
        public FsmEnum copyType;

        [ObjectType(typeof(ArrayPasteType))]
        public FsmEnum pasteType;

        [ActionSection("Result")]
        [Tooltip("Event sent if this array source does not contains that element (described below)")]
        public FsmEvent indexOutOfRange;

        public override void Reset()
        {
            this.arraySource = null;
            this.arrayTarget = null;
            this.indexToTransfer = null;
            this.copyType = ArrayTransferType.Copy;
            this.pasteType = ArrayPasteType.AsLastItem;
        }

        public override void OnEnter()
        {
            this.DoTransferValue();
            base.Finish();
        }

        private void DoTransferValue()
        {
            if (this.arraySource.IsNone || this.arrayTarget.IsNone)
            {
                return;
            }
            int value = this.indexToTransfer.Value;
            if (value < 0 || value >= this.arraySource.Length)
            {
                base.Fsm.Event(this.indexOutOfRange);
                return;
            }
            object obj = this.arraySource.Values[value];
            if ((ArrayTransferType)(object)this.copyType.Value == ArrayTransferType.Cut)
            {
                List<object> list = new List<object>(this.arraySource.Values);
                list.RemoveAt(value);
                this.arraySource.Values = list.ToArray();
            }
            else if ((ArrayTransferType)(object)this.copyType.Value == ArrayTransferType.nullify)
            {
                this.arraySource.Values.SetValue(null, value);
            }
            if ((ArrayPasteType)(object)this.pasteType.Value == ArrayPasteType.AsFirstItem)
            {
                List<object> list2 = new List<object>(this.arrayTarget.Values);
                list2.Insert(0, obj);
                this.arrayTarget.Values = list2.ToArray();
            }
            else if ((ArrayPasteType)(object)this.pasteType.Value == ArrayPasteType.AsLastItem)
            {
                this.arrayTarget.Resize(this.arrayTarget.Length + 1);
                this.arrayTarget.Set(this.arrayTarget.Length - 1, obj);
            }
            else if ((ArrayPasteType)(object)this.pasteType.Value == ArrayPasteType.InsertAtSameIndex)
            {
                if (value >= this.arrayTarget.Length)
                {
                    base.Fsm.Event(this.indexOutOfRange);
                }
                List<object> list3 = new List<object>(this.arrayTarget.Values);
                list3.Insert(value, obj);
                this.arrayTarget.Values = list3.ToArray();
            }
            else if ((ArrayPasteType)(object)this.pasteType.Value == ArrayPasteType.ReplaceAtSameIndex)
            {
                if (value >= this.arrayTarget.Length)
                {
                    base.Fsm.Event(this.indexOutOfRange);
                }
                else
                {
                    this.arrayTarget.Set(value, obj);
                }
            }
        }
    }
}
