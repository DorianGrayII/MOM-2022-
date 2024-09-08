namespace UnrealByte.EasyJira
{
    using LitJson;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class JIssueDetails
    {
        public string projectKey = "";
        public string projectTypeKey = "";
        public string projectName = "";
        public string projectIconURL = "";
        public Texture2D projectIconTexture;
        public string key = "";
        public string summary = "";
        public string type = "";
        public string typeIconURL = "";
        public Texture2D issueTypeIconTexture;
        public string priority = "";
        public string priorityIconURL = "";
        public Texture2D priorityIconTexture;
        public string affectVersions = "";
        public string components = "";
        public string labels = "";
        public string status = "";
        public string statusIconURL = "";
        public Texture2D statusIconTexture;
        public string resolution = "";
        public string fixVersions = "";
        public string assignee = "";
        public string reporter = "";
        public string votes = "";
        public string watcher = "";
        public string dateCreated = "";
        public string dateUpdated = "";
        public string description = "";
        public List<JIssueComment> comments = new List<JIssueComment>();
        public List<JIssueAttachment> attachments = new List<JIssueAttachment>();
        public List<JIssueChangelog> changelogHistories = new List<JIssueChangelog>();

        public JIssueDetails getIssueFromJsonString(string JSONString, JsonData JSONData)
        {
            JsonData data = null;
            data = (JSONString.Length <= 0) ? JSONData : JsonMapper.ToObject(JSONString);
            try
            {
                try
                {
                    this.projectKey = data["fields"]["project"]["key"].ToString();
                }
                catch (Exception)
                {
                    this.projectKey = "---";
                }
                goto TR_0082;
            TR_000C:
                try
                {
                    if (data["changelog"]["histories"].Count > 0)
                    {
                        for (int i = 0; i < data["changelog"]["histories"].Count; i++)
                        {
                            JIssueChangelog item = new JIssueChangelog();
                            try
                            {
                                item.getChangeLogFromJsonString(data["changelog"]["histories"][i].ToJson());
                                if (item != null)
                                {
                                    this.changelogHistories.Add(item);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                goto TR_0000;
            TR_0017:
                try
                {
                    if (data["fields"]["attachment"].Count > 0)
                    {
                        for (int i = 0; i < data["fields"]["attachment"].Count; i++)
                        {
                            JIssueAttachment item = new JIssueAttachment();
                            try
                            {
                                item.getIssueAttachmentFromJsonString(data["fields"]["attachment"][i].ToJson());
                                if (item != null)
                                {
                                    this.attachments.Add(item);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                goto TR_000C;
            TR_0023:
                try
                {
                    if ((data["fields"]["comment"].Count > 0) && (data["fields"]["comment"]["comments"].Count > 0))
                    {
                        for (int i = 0; i < data["fields"]["comment"]["comments"].Count; i++)
                        {
                            JIssueComment item = new JIssueComment();
                            try
                            {
                                item.getIssueCommentFromJsonString(data["fields"]["comment"]["comments"][i].ToJson());
                                if (item != null)
                                {
                                    this.comments.Add(item);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                goto TR_0017;
            TR_0026:
                try
                {
                    this.description = data["fields"]["description"].ToString();
                }
                catch (Exception)
                {
                    this.description = "(no description)";
                }
                goto TR_0023;
            TR_0029:
                try
                {
                    this.dateUpdated = DateTime.Parse(data["fields"]["updated"].ToString()).ToShortDateString();
                }
                catch (Exception)
                {
                    this.dateUpdated = "---";
                }
                goto TR_0026;
            TR_002C:
                try
                {
                    this.dateCreated = DateTime.Parse(data["fields"]["created"].ToString()).ToShortDateString();
                }
                catch (Exception)
                {
                    this.dateCreated = "---";
                }
                goto TR_0029;
            TR_002F:
                try
                {
                    this.votes = data["fields"]["votes"]["votes"].ToString();
                }
                catch (Exception)
                {
                    this.votes = "---";
                }
                goto TR_002C;
            TR_0032:
                try
                {
                    this.reporter = data["fields"]["reporter"]["name"].ToString();
                }
                catch (Exception)
                {
                    this.reporter = "---";
                }
                goto TR_002F;
            TR_0035:
                try
                {
                    this.assignee = data["fields"]["assignee"]["displayName"].ToString();
                }
                catch (Exception)
                {
                    this.assignee = "---";
                }
                goto TR_0032;
            TR_003D:
                try
                {
                    if (data["fields"]["fixVersions"].Count > 0)
                    {
                        for (int i = 0; i < data["fields"]["fixVersions"].Count; i++)
                        {
                            try
                            {
                                this.fixVersions = this.fixVersions + data["fields"]["fixVersions"][i]["name"].ToString() + " ";
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                goto TR_0035;
            TR_0045:
                try
                {
                    if (data["fields"]["versions"].Count > 0)
                    {
                        for (int i = 0; i < data["fields"]["versions"].Count; i++)
                        {
                            try
                            {
                                this.affectVersions = this.affectVersions + data["fields"]["versions"][i]["name"].ToString() + " ";
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                goto TR_003D;
            TR_0048:
                try
                {
                    this.resolution = DateTime.Parse(data["fields"]["resolutiondate"].ToString()).ToShortDateString();
                }
                catch (Exception)
                {
                    this.resolution = "---";
                }
                goto TR_0045;
            TR_004E:
                try
                {
                    string str3 = data["fields"]["status"]["iconUrl"].ToString();
                    if (str3.EndsWith(".svg"))
                    {
                        str3 = str3.Replace(".svg", ".png");
                    }
                    this.statusIconURL = str3;
                }
                catch (Exception)
                {
                    this.statusIconURL = "";
                }
                goto TR_0048;
            TR_0051:
                try
                {
                    this.status = data["fields"]["status"]["name"].ToString();
                }
                catch (Exception)
                {
                    this.status = "---";
                }
                goto TR_004E;
            TR_0059:
                try
                {
                    if (data["fields"]["labels"].Count > 0)
                    {
                        for (int i = 0; i < data["fields"]["labels"].Count; i++)
                        {
                            try
                            {
                                this.labels = this.labels + data["fields"]["labels"][i].ToString() + " ";
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                goto TR_0051;
            TR_0061:
                try
                {
                    if (data["fields"]["components"].Count > 0)
                    {
                        for (int i = 0; i < data["fields"]["components"].Count; i++)
                        {
                            try
                            {
                                this.components = this.components + data["fields"]["components"][i]["name"].ToString() + " ";
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                goto TR_0059;
            TR_0067:
                try
                {
                    string str2 = data["fields"]["priority"]["iconUrl"].ToString();
                    if (str2.EndsWith(".svg"))
                    {
                        str2 = str2.Replace(".svg", ".png");
                    }
                    this.priorityIconURL = str2;
                }
                catch (Exception)
                {
                    this.priorityIconURL = "";
                }
                goto TR_0061;
            TR_006A:
                try
                {
                    this.priority = data["fields"]["priority"]["name"].ToString();
                }
                catch (Exception)
                {
                    this.priority = "---";
                }
                goto TR_0067;
            TR_0070:
                try
                {
                    string str = data["fields"]["issuetype"]["iconUrl"].ToString();
                    if (str.EndsWith(".svg"))
                    {
                        str = str.Replace(".svg", ".png");
                    }
                    this.typeIconURL = str;
                }
                catch (Exception)
                {
                    this.typeIconURL = "";
                }
                goto TR_006A;
            TR_0073:
                try
                {
                    this.type = data["fields"]["issuetype"]["name"].ToString();
                }
                catch (Exception)
                {
                    this.type = "---";
                }
                goto TR_0070;
            TR_0076:
                try
                {
                    this.summary = data["fields"]["summary"].ToString();
                }
                catch (Exception)
                {
                    this.summary = "---";
                }
                goto TR_0073;
            TR_0079:
                try
                {
                    this.key = data["key"].ToString();
                }
                catch (Exception)
                {
                    this.key = "---";
                }
                goto TR_0076;
            TR_007C:
                try
                {
                    this.projectIconURL = data["fields"]["project"]["avatarUrls"]["16x16"].ToString();
                }
                catch (Exception)
                {
                    this.projectIconURL = "";
                }
                goto TR_0079;
            TR_007F:
                try
                {
                    this.projectName = data["fields"]["project"]["name"].ToString();
                }
                catch (Exception)
                {
                    this.projectName = "---";
                }
                goto TR_007C;
            TR_0082:
                try
                {
                    this.projectTypeKey = data["fields"]["project"]["projectTypeKey"].ToString();
                }
                catch (Exception)
                {
                    this.projectTypeKey = "---";
                }
                goto TR_007F;
            }
            catch (Exception exception1)
            {
                Debug.Log(exception1);
            }
        TR_0000:
            return this;
        }
    }
}

