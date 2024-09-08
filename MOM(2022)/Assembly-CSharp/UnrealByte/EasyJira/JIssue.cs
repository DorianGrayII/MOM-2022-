using System;

namespace UnrealByte.EasyJira
{
    [Serializable]
    public class JIssue
    {
        public JProject project;

        public string summary = "";

        public string description = "";

        public JIssueType issuetype;

        public JIssuePriority priority;

        public JIssue(string summary, string description)
        {
            this.summary = summary;
            this.description = description;
        }
    }
}
