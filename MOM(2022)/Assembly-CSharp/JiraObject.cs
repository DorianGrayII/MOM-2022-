// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// JiraObject
using System.Collections;
using System.Collections.Generic;
using MHUtils.UI;
using MOM;
using UnityEngine;
using UnrealByte.EasyJira;

public class JiraObject : MonoBehaviour
{
    public static JiraObject jiraInstance;

    private JiraConnect jiraConnect;

    [Header("Jira connection settings")]
    public string jiraBaseRestURL = "";

    public string jiraUser = "";

    public string jiraPassword = "";

    public string jiraProjectKey = "";

    [Space(10f)]
    [Header("Log settings")]
    public bool logActive;

    public bool logInCustomFile;

    public string logFilePath;

    public string initLogMessage;

    public bool debugLog;

    public bool includeWarnings;

    public bool takeScreenshot;

    private void Awake()
    {
        if (JiraObject.jiraInstance == null)
        {
            JiraObject.jiraInstance = this;
        }
        else if (JiraObject.jiraInstance != this)
        {
            Object.Destroy(base.gameObject);
        }
        Object.DontDestroyOnLoad(base.gameObject);
        if (this.logActive)
        {
            this.logFilePath = TLog.Get().logFilePath;
        }
    }

    public IEnumerator Start()
    {
        if (this.jiraUser.Length == 0)
        {
            Debug.Log("[EasyJira] Please set the Jira user in the Jira object");
            yield break;
        }
        if (this.jiraPassword.Length == 0)
        {
            Debug.Log("[EasyJira] Please set the Jira password in the Jira object");
            yield break;
        }
        if (this.jiraBaseRestURL.Length == 0)
        {
            Debug.Log("[EasyJira] Please set the Jira Base Rest URL in the Jira object");
            yield break;
        }
        if (this.jiraProjectKey.Length == 0)
        {
            Debug.Log("[EasyJira] Please set the Jira Project Key.");
            yield break;
        }
        if (this.jiraBaseRestURL.EndsWith("/"))
        {
            this.jiraBaseRestURL = this.jiraBaseRestURL.Substring(0, this.jiraBaseRestURL.Length - 1);
        }
        JiraSettings jiraSettings = ScriptableObject.CreateInstance("JiraSettings") as JiraSettings;
        jiraSettings.Init(this.jiraBaseRestURL, this.jiraUser, this.jiraPassword, this.jiraProjectKey);
        this.jiraConnect = new JiraConnect();
        yield return this.jiraConnect.APIDownloadProjectIssueTypesIG(jiraSettings, this.jiraProjectKey, subTask: false);
        if (this.jiraConnect.jIssueTypes != null)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < this.jiraConnect.jIssueTypes.Count; i++)
            {
                list.Add(this.jiraConnect.jIssueTypes[i].name);
            }
        }
    }

    public void SendForm(string title, string description)
    {
        int num = 0;
        if (this.jiraConnect.jIssueTypes != null && this.jiraConnect.jIssueTypes.Count > 0)
        {
            JIssueType jIssueType = this.jiraConnect.jIssueTypes.Find((JIssueType o) => o.name == "Bug");
            if (jIssueType == null)
            {
                jIssueType = this.jiraConnect.jIssueTypes[0];
            }
            num = jIssueType.id;
            this.SendIssueForm(title, description, num, "3");
        }
        else
        {
            Debug.LogError("jira types not downloaded");
        }
    }

    public Coroutine SendIssueForm(string title, string description, int issueType, string priority)
    {
        string text = "";
        text = JiraConnect.GenerateBasicAuth(this.jiraUser, this.jiraPassword);
        JIssue jIssue = new JIssue(title, description);
        jIssue.issuetype = new JIssueType(issueType);
        jIssue.priority = new JIssuePriority(priority);
        return base.StartCoroutine(this.SendIssuePost(text, jIssue));
    }

    public IEnumerator SendIssuePost(string encodedAuth, JIssue jIssue)
    {
        Debug.Log("[EasyJira] SendIssueForm");
        if (!this.logActive)
        {
            this.logFilePath = null;
        }
        JiraSettings jiraSettings = ScriptableObject.CreateInstance("JiraSettings") as JiraSettings;
        jiraSettings.Init(this.jiraBaseRestURL, this.jiraUser, this.jiraPassword, this.jiraProjectKey);
        yield return this.jiraConnect.APISendIssueIG(jiraSettings, jIssue, this.logFilePath, this.takeScreenshot);
        yield return new WaitForSeconds(1f);
        BugReport screen = UIManager.GetScreen<BugReport>(UIManager.Layer.Popup);
        if (screen != null)
        {
            UIManager.Close(screen);
        }
    }

    public IEnumerator ActivateGO(GameObject gameObject, float timeInSeconds, bool setActive = true)
    {
        gameObject.SetActive(setActive);
        yield return new WaitForSeconds(timeInSeconds);
        gameObject.SetActive(!setActive);
    }
}
