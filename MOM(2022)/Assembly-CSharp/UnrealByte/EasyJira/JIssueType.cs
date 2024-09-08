namespace UnrealByte.EasyJira
{
    using System;

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

