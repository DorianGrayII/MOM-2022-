using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;
using MHUtils;
using MOM;
using UnityEngine;
using UnityEngine.Networking;

namespace UnrealByte.EasyJira
{
    public class JiraConnect
    {
        public static int uploadProgress;

        public const string fileNameExt = "jpg";

        public const string fileName = "screenshot.jpg";

        public List<JIssueType> jIssueTypes;

        public static Action<string> callbackAction;

        public IEnumerator APISendIssueIG(JiraSettings settings, JIssue issue, string logFilePath, bool takeScreenshot)
        {
            JiraConnect.uploadProgress = 5;
            string encodedCredentials = JiraConnect.GenerateBasicAuth(settings.jiraUser, settings.jiraToken);
            string jiraIssueURL = settings.getJiraIssueURL();
            issue.project = new JProject(settings.jiraProjectKey);
            JsonData jsonData = new JsonData("{\"fields\":" + JsonUtility.ToJson(issue).ToString() + "}");
            byte[] bytes = Encoding.UTF8.GetBytes(jsonData.ToString());
            UnityWebRequest request = new UnityWebRequest(jiraIssueURL, "POST");
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Basic " + encodedCredentials);
            request.SetRequestHeader("Content-Type", "application/json");
            JiraConnect.uploadProgress = 10;
            yield return request.SendWebRequest();
            JiraConnect.uploadProgress = 20;
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("[EasyJira] " + request.responseCode + " - " + request.downloadHandler.text);
            }
            else
            {
                Debug.Log("[EasyJira] Feedback post complete!");
                JsonData jsonData2 = JsonMapper.ToObject(request.downloadHandler.text);
                string key = jsonData2["key"].ToString();
                if (takeScreenshot)
                {
                    byte[] screenShot = BugReportCatcher.screenShot;
                    WWWForm wWWForm = new WWWForm();
                    wWWForm.AddBinaryData("file", screenShot, key, "image/jpg");
                    UnityWebRequest requestAttach5 = UnityWebRequest.Post(jiraIssueURL + "/" + key + "/attachments", wWWForm);
                    requestAttach5.SetRequestHeader("X-Atlassian-Token", "no-check");
                    requestAttach5.SetRequestHeader("Authorization", "Basic " + encodedCredentials);
                    yield return requestAttach5.SendWebRequest();
                    if (requestAttach5.isNetworkError || requestAttach5.isHttpError)
                    {
                        Debug.Log("[EasyJira] " + requestAttach5.responseCode + " - " + requestAttach5.downloadHandler.text);
                    }
                    else
                    {
                        Debug.Log("[EasyJira] Screenshot Attach complete!");
                    }
                    requestAttach5.Dispose();
                }
                JiraConnect.uploadProgress = 30;
                string path = MHApplication.PROFILES;
                string fullPath2 = Path.Combine(path, "Autosave_1");
                if (File.Exists(fullPath2 + ".save"))
                {
                    byte[] contents = File.ReadAllBytes(fullPath2 + ".save");
                    WWWForm wWWForm2 = new WWWForm();
                    wWWForm2.AddBinaryData("file", contents, key + "_save.save");
                    UnityWebRequest requestAttach5 = UnityWebRequest.Post(jiraIssueURL + "/" + key + "/attachments", wWWForm2);
                    requestAttach5.SetRequestHeader("X-Atlassian-Token", "no-check");
                    requestAttach5.SetRequestHeader("Authorization", "Basic " + encodedCredentials);
                    yield return requestAttach5.SendWebRequest();
                    if (requestAttach5.isNetworkError || requestAttach5.isHttpError)
              //      if (requestAttach5.isNetworkError || requestAttach5.isHttpError)
                    {
                        Debug.Log("[EasyJira] " + requestAttach5.responseCode + " - " + requestAttach5.downloadHandler.text);
                    }
                    else
                    {
                        Debug.Log("[EasyJira] Save Base Attach complete!");
                    }
                    requestAttach5.Dispose();
                }
                JiraConnect.uploadProgress = 50;
                if (File.Exists(fullPath2 + ".metasave"))
                {
                    byte[] contents2 = File.ReadAllBytes(fullPath2 + ".metasave");
                    WWWForm wWWForm3 = new WWWForm();
                    wWWForm3.AddBinaryData("file", contents2, key + "_save.metasave");
                    UnityWebRequest requestAttach5 = UnityWebRequest.Post(jiraIssueURL + "/" + key + "/attachments", wWWForm3);
                    requestAttach5.SetRequestHeader("X-Atlassian-Token", "no-check");
                    requestAttach5.SetRequestHeader("Authorization", "Basic " + encodedCredentials);
                    yield return requestAttach5.SendWebRequest();
                    if (requestAttach5.isNetworkError || requestAttach5.isHttpError)
                  //  if (requestAttach5.isNetworkError || requestAttach5.isHttpError)
                    {
                        Debug.Log("[EasyJira] " + requestAttach5.responseCode + " - " + requestAttach5.downloadHandler.text);
                    }
                    else
                    {
                        Debug.Log("[EasyJira] Save Meta Attach complete!");
                    }
                    requestAttach5.Dispose();
                }
                JiraConnect.uploadProgress = 60;
                fullPath2 = Path.Combine(path, "Autosave_2");
                if (File.Exists(fullPath2 + ".save"))
                {
                    byte[] contents3 = File.ReadAllBytes(fullPath2 + ".save");
                    WWWForm wWWForm4 = new WWWForm();
                    wWWForm4.AddBinaryData("file", contents3, key + "_save_.save");
                    UnityWebRequest requestAttach5 = UnityWebRequest.Post(jiraIssueURL + "/" + key + "/attachments", wWWForm4);
                    requestAttach5.SetRequestHeader("X-Atlassian-Token", "no-check");
                    requestAttach5.SetRequestHeader("Authorization", "Basic " + encodedCredentials);
                    yield return requestAttach5.SendWebRequest();
                    if (requestAttach5.isNetworkError || requestAttach5.isHttpError)
                    {
                        Debug.Log("[EasyJira] " + requestAttach5.responseCode + " - " + requestAttach5.downloadHandler.text);
                    }
                    else
                    {
                        Debug.Log("[EasyJira] Save Base Attach complete!");
                    }
                    requestAttach5.Dispose();
                }
                JiraConnect.uploadProgress = 80;
                if (File.Exists(fullPath2 + ".metasave"))
                {
                    byte[] contents4 = File.ReadAllBytes(fullPath2 + ".metasave");
                    WWWForm wWWForm5 = new WWWForm();
                    wWWForm5.AddBinaryData("file", contents4, key + "_save_.metasave");
                    UnityWebRequest requestAttach5 = UnityWebRequest.Post(jiraIssueURL + "/" + key + "/attachments", wWWForm5);
                    requestAttach5.SetRequestHeader("X-Atlassian-Token", "no-check");
                    requestAttach5.SetRequestHeader("Authorization", "Basic " + encodedCredentials);
                    yield return requestAttach5.SendWebRequest();
                    if (requestAttach5.isNetworkError || requestAttach5.isHttpError)
                    {
                        Debug.Log("[EasyJira] " + requestAttach5.responseCode + " - " + requestAttach5.downloadHandler.text);
                    }
                    else
                    {
                        Debug.Log("[EasyJira] Save Meta Attach complete!");
                    }
                    requestAttach5.Dispose();
                }
                JiraConnect.uploadProgress = 90;
                if (logFilePath != null)
                {
                    Debug.Log(logFilePath);
                    byte[] contents5 = File.ReadAllBytes(logFilePath);
                    WWWForm wWWForm6 = new WWWForm();
                    wWWForm6.AddBinaryData("file", contents5, key + "_log.txt");
                    UnityWebRequest requestAttach5 = UnityWebRequest.Post(jiraIssueURL + "/" + key + "/attachments", wWWForm6);
                    requestAttach5.SetRequestHeader("X-Atlassian-Token", "no-check");
                    requestAttach5.SetRequestHeader("Authorization", "Basic " + encodedCredentials);
                    yield return requestAttach5.SendWebRequest();
                    if (requestAttach5.isNetworkError || requestAttach5.isHttpError)
                    {
                        Debug.Log("[EasyJira] " + requestAttach5.responseCode + " - " + requestAttach5.downloadHandler.text);
                    }
                    else
                    {
                        Debug.Log("[EasyJira] LogFile Attach complete!");
                    }
                    requestAttach5.Dispose();
                }
            }
            request.Dispose();
            JiraConnect.uploadProgress = 100;
        }

        public IEnumerator APIDownloadProjectIssueTypesIG(JiraSettings settings, string projectKey, bool subTask)
        {
            string text = JiraConnect.GenerateBasicAuth(settings.jiraUser, settings.jiraToken);
            string url = settings.getJiraProjectURL() + "/" + projectKey;
            UnityWebRequest request = new UnityWebRequest(url, "GET");
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Basic " + text);
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("[EasyJira] " + request.responseCode + " - " + request.downloadHandler.text);
                yield break;
            }
            JsonData jsonData = JsonMapper.ToObject(request.downloadHandler.text);
            if (jsonData["issueTypes"].Count <= 0)
            {
                yield break;
            }
            this.jIssueTypes = new List<JIssueType>();
            for (int i = 0; i < jsonData["issueTypes"].Count; i++)
            {
                if (!subTask)
                {
                    try
                    {
                        if (bool.Parse(jsonData["issueTypes"][i]["subtask"].ToString()))
                        {
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.Message);
                    }
                }
                JIssueType jIssueType = new JIssueType();
                jIssueType.name = jsonData["issueTypes"][i]["name"].ToString();
                jIssueType.id = int.Parse(jsonData["issueTypes"][i]["id"].ToString());
                this.jIssueTypes.Add(jIssueType);
            }
        }

        public static IEnumerator APITestAuth(JiraSettings settings, Action<string> callback)
        {
            string encodedCredentials = JiraConnect.GenerateBasicAuth(settings.jiraUser, settings.jiraToken);
            string jiraMyselfURL = settings.getJiraMyselfURL();
            UnityWebRequest www2 = new UnityWebRequest(jiraMyselfURL, "GET");
            www2.downloadHandler = new DownloadHandlerBuffer();
            www2.SetRequestHeader("Authorization", "Basic " + encodedCredentials);
            www2.SetRequestHeader("Content-Type", "application/json");
            www2.SetRequestHeader("Accept", "application/json");
            yield return www2.SendWebRequest();
            if (www2.isNetworkError || www2.isHttpError)
            {
                Debug.LogError("[EasyJira] " + www2.error);
                yield break;
            }
            JsonData jsonData = JsonMapper.ToObject(www2.downloadHandler.text);
            if (jsonData != null)
            {
                settings.jiraName = jsonData["name"].ToString();
                settings.jiraAccountId = jsonData["accountId"].ToString();
            }
            www2 = new UnityWebRequest(settings.getJiraMyPermissionsURL() + "?permissions=ASSIGN_ISSUES", "GET");
            www2.downloadHandler = new DownloadHandlerBuffer();
            www2.SetRequestHeader("Authorization", "Basic " + encodedCredentials);
            www2.SetRequestHeader("Content-Type", "application/json");
            www2.SetRequestHeader("Accept", "application/json");
            yield return www2.SendWebRequest();
            if (www2.isNetworkError || www2.isHttpError)
            {
                Debug.LogError("[EasyJira] " + www2.error);
            }
            else
            {
                try
                {
                    jsonData = JsonMapper.ToObject(www2.downloadHandler.text);
                    if (jsonData != null)
                    {
                        settings.assignIssues = bool.Parse(jsonData["permissions"]["ASSIGN_ISSUES"]["havePermission"].ToString());
                    }
                }
                catch (Exception)
                {
                    Debug.LogError("[EasyJira] " + www2.error);
                }
            }
            callback("[EasyJira] Success! Settings saved");
        }

        public static IEnumerator APICreateIssue(JiraSettings settings, JIssue issue, Action<string> callback)
        {
            string text = JiraConnect.GenerateBasicAuth(settings.jiraUser, settings.jiraToken);
            string jiraIssueURL = settings.getJiraIssueURL();
            issue.project = new JProject(settings.jiraProjectKey);
            JsonData jsonData = new JsonData("{\"fields\":" + JsonUtility.ToJson(issue).ToString() + "}");
            byte[] bytes = Encoding.UTF8.GetBytes(jsonData.ToString());
            UnityWebRequest www = new UnityWebRequest(jiraIssueURL, "POST");
            www.uploadHandler = new UploadHandlerRaw(bytes);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Authorization", "Basic " + text);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();
            try
            {
                if (!www.isNetworkError && !www.isHttpError)
                {
                    callback(www.downloadHandler.text);
                }
                else
                {
                    callback("error: " + www.downloadHandler.text);
                }
            }
            catch (Exception)
            {
            }
            yield return new WaitForSeconds(2f);
        }

        public static IEnumerator APIDownloadProjectIssueTypes(JiraSettings settings, string projectKey, bool subTask, Action<List<JIssueType>> callback)
        {
            string text = JiraConnect.GenerateBasicAuth(settings.jiraUser, settings.jiraToken);
            string url = settings.getJiraProjectURL() + "/" + projectKey;
            List<JIssueType> issueTypes = new List<JIssueType>();
            UnityWebRequest www = new UnityWebRequest(url, "GET");
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Authorization", "Basic " + text);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();
            try
            {
                JsonData jsonData = JsonMapper.ToObject(www.downloadHandler.text);
                if (jsonData["issueTypes"].Count > 0)
                {
                    for (int i = 0; i < jsonData["issueTypes"].Count; i++)
                    {
                        if (!subTask)
                        {
                            try
                            {
                                if (bool.Parse(jsonData["issueTypes"][i]["subtask"].ToString()))
                                {
                                    continue;
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        JIssueType jIssueType = new JIssueType();
                        jIssueType.name = jsonData["issueTypes"][i]["name"].ToString();
                        jIssueType.id = int.Parse(jsonData["issueTypes"][i]["id"].ToString());
                        issueTypes.Add(jIssueType);
                    }
                }
                callback(issueTypes);
            }
            catch (Exception)
            {
            }
            yield return new WaitForSeconds(2f);
        }

        public static IEnumerator APIDownloadPriorities(JiraSettings settings, Action<List<JIssuePriority>> callback)
        {
            string text = JiraConnect.GenerateBasicAuth(settings.jiraUser, settings.jiraToken);
            string jiraPrioritiesURL = settings.getJiraPrioritiesURL();
            List<JIssuePriority> priorities = new List<JIssuePriority>();
            UnityWebRequest www = new UnityWebRequest(jiraPrioritiesURL, "GET");
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Authorization", "Basic " + text);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();
            try
            {
                JsonData jsonData = JsonMapper.ToObject(www.downloadHandler.text);
                if (jsonData.Count > 0)
                {
                    for (int i = 0; i < jsonData.Count; i++)
                    {
                        JIssuePriority jIssuePriority = new JIssuePriority();
                        jIssuePriority.name = jsonData[i]["name"].ToString();
                        jIssuePriority.id = jsonData[i]["id"].ToString();
                        priorities.Add(jIssuePriority);
                    }
                }
                callback(priorities);
            }
            catch (Exception)
            {
            }
            yield return new WaitForSeconds(2f);
        }

        public static IEnumerator APIDownloadUsersAssignableToIssues(JiraSettings settings, string query, string issueKey, Action<List<JUser>> callback)
        {
            string text = JiraConnect.GenerateBasicAuth(settings.jiraUser, settings.jiraToken);
            string url = settings.getJiraUserAssignableIssuesURL() + "?query=" + query + "&issueKey=" + issueKey;
            UnityWebRequest www = new UnityWebRequest(url, "GET");
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Authorization", "Basic " + text);
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");
            yield return www.SendWebRequest();
            try
            {
                if (!www.isNetworkError && !www.isHttpError)
                {
                    List<JUser> list = new List<JUser>();
                    JsonData jsonData = JsonMapper.ToObject(www.downloadHandler.text);
                    if (jsonData.Count > 0)
                    {
                        for (int i = 0; i < jsonData.Count; i++)
                        {
                            JUser jUser = new JUser();
                            jUser.accountId = jsonData[i]["accountId"].ToString();
                            jUser.emailAddress = jsonData[i]["emailAddress"].ToString();
                            jUser.displayName = jsonData[i]["displayName"].ToString();
                            jUser.avatarURL = jsonData[i]["avatarUrls"]["24x24"].ToString();
                            list.Add(jUser);
                        }
                    }
                    callback(list);
                }
                else
                {
                    Debug.LogError("error: " + www.downloadHandler.text);
                }
            }
            catch (Exception message)
            {
                Debug.LogError(message);
            }
            yield return new WaitForSeconds(2f);
        }

        public static IEnumerator APIGetIssue(JiraSettings settings, string jIssueKey, Action<string> callback)
        {
            string text = JiraConnect.GenerateBasicAuth(settings.jiraUser, settings.jiraToken);
            string url = settings.getJiraIssueURL() + "/" + jIssueKey + "?expand=operations,changelog";
            UnityWebRequest www = new UnityWebRequest(url, "GET");
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Authorization", "Basic " + text);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();
            try
            {
                callback(www.downloadHandler.text);
            }
            catch (Exception)
            {
            }
            yield return new WaitForSeconds(2f);
        }

        public static IEnumerator APISearchIssues(JiraSettings settings, int startAt, int maxResults, Action<string> callback)
        {
            string text = JiraConnect.GenerateBasicAuth(settings.jiraUser, settings.jiraToken);
            JsonData jsonData = new JsonData("{\"jql\":\"project = " + settings.jiraProjectKey + "\", \"startAt\":" + startAt + ",\"maxResults\":" + maxResults + ", \"fields\":[\"id\",\"key\",\"summary\",\"issuetype\",\"status\",\"assignee\",\"priority\",\"duedate\"]}");
            byte[] bytes = Encoding.UTF8.GetBytes(jsonData.ToString());
            UnityWebRequest www = new UnityWebRequest(settings.getJiraSearchURL(), "POST");
            www.uploadHandler = new UploadHandlerRaw(bytes);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Authorization", "Basic " + text);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();
            try
            {
                if (!www.isNetworkError && !www.isHttpError)
                {
                    callback(www.downloadHandler.text);
                }
                else
                {
                    callback("error: " + www.downloadHandler.text);
                }
            }
            catch (Exception)
            {
            }
            yield return new WaitForSeconds(2f);
        }

        public static IEnumerator APIAssignIssue(JiraSettings settings, string issueKey, string accountId, Action<string> callback)
        {
            string text = JiraConnect.GenerateBasicAuth(settings.jiraUser, settings.jiraToken);
            string url = settings.getJiraIssueURL() + "/" + issueKey;
            JsonData jsonData = new JsonData("{\"fields\": {\"assignee\":{\"accountId\":\"" + accountId + "\"}}}");
            byte[] bytes = Encoding.UTF8.GetBytes(jsonData.ToString());
            UnityWebRequest www = new UnityWebRequest(url, "PUT");
            www.uploadHandler = new UploadHandlerRaw(bytes);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Authorization", "Basic " + text);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();
            try
            {
                if (!www.isNetworkError && !www.isHttpError)
                {
                    callback(www.downloadHandler.text);
                }
                else
                {
                    callback("error: " + www.downloadHandler.text);
                }
            }
            catch (Exception)
            {
            }
            yield return new WaitForSeconds(2f);
        }

        public static IEnumerator APIAddComment(string comment, string IssueKey, JiraSettings settings, Action<string> callback)
        {
            string text = JiraConnect.GenerateBasicAuth(settings.jiraUser, settings.jiraToken);
            string jiraIssueURL = settings.getJiraIssueURL();
            jiraIssueURL = jiraIssueURL + "/" + IssueKey + "/comment";
            JsonData jsonData = new JsonData("{\"body\":\"" + comment + "\"}");
            byte[] bytes = Encoding.UTF8.GetBytes(jsonData.ToString());
            UnityWebRequest www = new UnityWebRequest(jiraIssueURL, "POST");
            www.uploadHandler = new UploadHandlerRaw(bytes);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Authorization", "Basic " + text);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();
            try
            {
                callback(www.downloadHandler.text);
            }
            catch (Exception)
            {
            }
            yield return new WaitForSeconds(2f);
        }

        public static string GenerateBasicAuth(string jiraUser, string jiraPassword)
        {
            string s = $"{jiraUser}:{jiraPassword}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
        }

        public static IEnumerator GetTexture(string url, Action<Texture2D> result)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();
            try
            {
                result(DownloadHandlerTexture.GetContent(www));
            }
            catch (Exception)
            {
            }
            yield return new WaitForSeconds(2f);
        }
    }
}
