using System;
using UnityEngine;

namespace UnrealByte.EasyJira
{
    [Serializable]
    public class JiraSettings : ScriptableObject
    {
        public string jiraBaseRestURL = "https://EXAMPLE.atlassian.net";

        [HideInInspector]
        public string jiraMyselfURL = "/rest/api/2/myself";

        [HideInInspector]
        public string jiraUserURL = "/rest/api/2/user";

        [HideInInspector]
        public string jiraStatusURL = "/rest/api/2/status";

        [HideInInspector]
        public string jiraProjectURL = "/rest/api/2/project";

        [HideInInspector]
        public string jiraIssueURL = "/rest/api/2/issue";

        [HideInInspector]
        public string jiraIssueTypeURL = "/rest/api/2/issuetype";

        [HideInInspector]
        public string jiraPrioritiesURL = "/rest/api/2/priority";

        [HideInInspector]
        public string jiraSearchURL = "/rest/api/2/search";

        [HideInInspector]
        public string jiraAccountId = "account-id";

        [HideInInspector]
        public string jiraName = "name";

        [HideInInspector]
        public string jiraUser = "username";

        [HideInInspector]
        public string jiraToken = "api-token";

        [HideInInspector]
        public string jiraProjectKey = "";

        public int maxResults = 50;

        public int maxShowAttachments = 5;

        public bool showAttachAtInit;

        public bool showCommentsAtInit;

        public bool showHistoryAtInit;

        [HideInInspector]
        public bool assignIssues;

        public JiraSettings Init(string jiraBaseRestURL, string jiraUser, string jiraToken, string jiraProjectKey)
        {
            this.jiraBaseRestURL = jiraBaseRestURL;
            this.jiraUser = jiraUser;
            this.jiraToken = jiraToken;
            this.jiraProjectKey = jiraProjectKey;
            return this;
        }

        public JiraSettings Init(string jiraBaseRestURL, string jiraUser, string jiraToken, string jiraProjectKey, int maxResults, bool showAttach, bool showComments, bool showHistory)
        {
            this.jiraBaseRestURL = jiraBaseRestURL;
            this.jiraUser = jiraUser;
            this.jiraToken = jiraToken;
            this.jiraProjectKey = jiraProjectKey;
            this.maxResults = maxResults;
            this.showAttachAtInit = showAttach;
            this.showCommentsAtInit = showComments;
            this.showHistoryAtInit = showHistory;
            return this;
        }

        public void sanitizeBaseURL()
        {
            if (this.jiraBaseRestURL.EndsWith("/"))
            {
                this.jiraBaseRestURL = this.jiraBaseRestURL.Substring(0, this.jiraBaseRestURL.Length - 1);
            }
        }

        public string getJiraMyselfURL()
        {
            this.sanitizeBaseURL();
            return this.jiraBaseRestURL + this.jiraMyselfURL;
        }

        public string getJiraUserURL()
        {
            this.sanitizeBaseURL();
            return this.jiraBaseRestURL + this.jiraUserURL;
        }

        public string getJiraStatusURL()
        {
            this.sanitizeBaseURL();
            return this.jiraBaseRestURL + this.jiraStatusURL;
        }

        public string getJiraProjectURL()
        {
            this.sanitizeBaseURL();
            return this.jiraBaseRestURL + this.jiraProjectURL;
        }

        public string getJiraIssueURL()
        {
            this.sanitizeBaseURL();
            return this.jiraBaseRestURL + this.jiraIssueURL;
        }

        public string getJiraIssueTypeURL()
        {
            this.sanitizeBaseURL();
            return this.jiraBaseRestURL + this.jiraIssueTypeURL;
        }

        public string getJiraPrioritiesURL()
        {
            this.sanitizeBaseURL();
            return this.jiraBaseRestURL + this.jiraPrioritiesURL;
        }

        public string getJiraSearchURL()
        {
            this.sanitizeBaseURL();
            return this.jiraBaseRestURL + this.jiraSearchURL;
        }

        public string getJiraUserAssignableIssuesURL()
        {
            this.sanitizeBaseURL();
            return this.jiraBaseRestURL + this.jiraUserURL + "/assignable/search";
        }

        public string getJiraMyPermissionsURL()
        {
            this.sanitizeBaseURL();
            return this.jiraBaseRestURL + "/rest/api/2/mypermissions";
        }
    }
}
