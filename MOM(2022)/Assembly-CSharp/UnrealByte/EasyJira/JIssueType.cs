using System;

namespace UnrealByte.EasyJira
{
    [Serializable]
    public class JIssueType
    {
        public int id;

        [NonSerialized]
        public string name;

        public JIssueType()
        {
        }

        public JIssueType(int id)
        {
            this.id = id;
        }
    }
}
