using System;

namespace MHUtils
{
    public class BasicTaskFunc : ITask
    {
        public Func<object, object> ca;

        public object data;

        public object Execute()
        {
            return this.ca(this.data);
        }
    }
}
