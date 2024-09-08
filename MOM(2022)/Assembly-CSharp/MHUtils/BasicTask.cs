namespace MHUtils
{
    public class BasicTask : ITask
    {
        public CallbackRet ca;

        public object data;

        public object Execute()
        {
            return this.ca(this.data);
        }
    }
}
