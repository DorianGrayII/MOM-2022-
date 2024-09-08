using System;

namespace UnrealByte.EasyJira
{
    [Serializable]
    public class JIssuePriority
    {
        public string id;

        [NonSerialized]
        public string name;

        public JIssuePriority()
        {
        }

        public JIssuePriority(string id)
        {
            this.id = id;
        }
    }
}
