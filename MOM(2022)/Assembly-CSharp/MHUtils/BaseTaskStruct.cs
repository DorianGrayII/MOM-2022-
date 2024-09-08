using System;

namespace MHUtils
{
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
