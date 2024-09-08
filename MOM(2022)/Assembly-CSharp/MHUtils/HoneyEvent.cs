namespace MHUtils
{
    public class HoneyEvent
    {
        public event EventFunction handler;

        public void TriggerEvent(object sender, object args)
        {
            if (this.handler != null)
            {
                this.handler(sender, args);
            }
        }
    }
}
