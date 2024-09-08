namespace MHUtils
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct BaseTaskStruct
    {
        public Func<object, object> ca;
        public object data;
        public object Execute()
        {
            return this.ca(this.data);
        }
    }
}

