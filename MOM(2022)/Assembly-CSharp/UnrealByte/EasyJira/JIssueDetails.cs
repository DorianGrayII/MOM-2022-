using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace UnrealByte.EasyJira
{
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
            JsonData jsonData = null;
            jsonData = ((JSONString.Length <= 0) ? JSONData : JsonMapper.ToObject(JSONString));
            try
            {
                try
                {
                    this.projectKey = jsonData["fields"]["project"]["key"].ToString();
                }
                catch (Exception)
                {
                    this.projectKey = "---";
                }
                try
                {
                    this.projectTypeKey = jsonData["fields"]["project"]["projectTypeKey"].ToString();
                }
                catch (Exception)
                {
                    this.projectTypeKey = "---";
                }
                try
                {
                    this.projectName = jsonData["fields"]["project"]["name"].ToString();
                }
                catch (Exception)
                {
                    this.projectName = "---";
                }
                try
                {
                    this.projectIconURL = jsonData["fields"]["project"]["avatarUrls"]["16x16"].ToString();
                }
                catch (Exception)
                {
                    this.projectIconURL = "";
                }
                try
                {
                    this.key = jsonData["key"].ToString();
                }
                catch (Exception)
                {
                    this.key = "---";
                }
                try
                {
                    this.summary = jsonData["fields"]["summary"].ToString();
                }
                catch (Exception)
                {
                    this.summary = "---";
                }
                try
                {
                    this.type = jsonData["fields"]["issuetype"]["name"].ToString();
                }
                catch (Exception)
                {
                    this.type = "---";
                }
                try
                {
                    string text = jsonData["fields"]["issuetype"]["iconUrl"].ToString();
                    if (text.EndsWith(".svg"))
                    {
                        text = text.Replace(".svg", ".png");
                    }
                    this.typeIconURL = text;
                }
                catch (Exception)
                {
                    this.typeIconURL = "";
                }
                try
                {
                    this.priority = jsonData["fields"]["priority"]["name"].ToString();
                }
                catch (Exception)
                {
                    this.priority = "---";
                }
                try
                {
                    string text2 = jsonData["fields"]["priority"]["iconUrl"].ToString();
                    if (text2.EndsWith(".svg"))
                    {
                        text2 = text2.Replace(".svg", ".png");
                    }
                    this.priorityIconURL = text2;
                }
                catch (Exception)
                {
                    this.priorityIconURL = "";
                }
                try
                {
                    if (jsonData["fields"]["components"].Count > 0)
                    {
                        for (int i = 0; i < jsonData["fields"]["components"].Count; i++)
                        {
                            try
                            {
                                this.components = this.components + jsonData["fields"]["components"][i]["name"].ToString() + " ";
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
                try
                {
                    if (jsonData["fields"]["labels"].Count > 0)
                    {
                        for (int j = 0; j < jsonData["fields"]["labels"].Count; j++)
                        {
                            try
                            {
                                this.labels = this.labels + jsonData["fields"]["labels"][j].ToString() + " ";
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
                try
                {
                    this.status = jsonData["fields"]["status"]["name"].ToString();
                }
                catch (Exception)
                {
                    this.status = "---";
                }
                try
                {
                    string text3 = jsonData["fields"]["status"]["iconUrl"].ToString();
                    if (text3.EndsWith(".svg"))
                    {
                        text3 = text3.Replace(".svg", ".png");
                    }
                    this.statusIconURL = text3;
                }
                catch (Exception)
                {
                    this.statusIconURL = "";
                }
                try
                {
                    this.resolution = DateTime.Parse(jsonData["fields"]["resolutiondate"].ToString()).ToShortDateString();
                }
                catch (Exception)
                {
                    this.resolution = "---";
                }
                try
                {
                    if (jsonData["fields"]["versions"].Count > 0)
                    {
                        for (int k = 0; k < jsonData["fields"]["versions"].Count; k++)
                        {
                            try
                            {
                                this.affectVersions = this.affectVersions + jsonData["fields"]["versions"][k]["name"].ToString() + " ";
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
                try
                {
                    if (jsonData["fields"]["fixVersions"].Count > 0)
                    {
                        for (int l = 0; l < jsonData["fields"]["fixVersions"].Count; l++)
                        {
                            try
                            {
                                this.fixVersions = this.fixVersions + jsonData["fields"]["fixVersions"][l]["name"].ToString() + " ";
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
                try
                {
                    this.assignee = jsonData["fields"]["assignee"]["displayName"].ToString();
                }
                catch (Exception)
                {
                    this.assignee = "---";
                }
                try
                {
                    this.reporter = jsonData["fields"]["reporter"]["name"].ToString();
                }
                catch (Exception)
                {
                    this.reporter = "---";
                }
                try
                {
                    this.votes = jsonData["fields"]["votes"]["votes"].ToString();
                }
                catch (Exception)
                {
                    this.votes = "---";
                }
                try
                {
                    this.dateCreated = DateTime.Parse(jsonData["fields"]["created"].ToString()).ToShortDateString();
                }
                catch (Exception)
                {
                    this.dateCreated = "---";
                }
                try
                {
                    this.dateUpdated = DateTime.Parse(jsonData["fields"]["updated"].ToString()).ToShortDateString();
                }
                catch (Exception)
                {
                    this.dateUpdated = "---";
                }
                try
                {
                    this.description = jsonData["fields"]["description"].ToString();
                }
                catch (Exception)
                {
                    this.description = "(no description)";
                }
                try
                {
                    if (jsonData["fields"]["comment"].Count > 0 && jsonData["fields"]["comment"]["comments"].Count > 0)
                    {
                        for (int m = 0; m < jsonData["fields"]["comment"]["comments"].Count; m++)
                        {
                            JIssueComment jIssueComment = new JIssueComment();
                            try
                            {
                                jIssueComment.getIssueCommentFromJsonString(jsonData["fields"]["comment"]["comments"][m].ToJson());
                                if (jIssueComment != null)
                                {
                                    this.comments.Add(jIssueComment);
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
                try
                {
                    if (jsonData["fields"]["attachment"].Count > 0)
                    {
                        for (int n = 0; n < jsonData["fields"]["attachment"].Count; n++)
                        {
                            JIssueAttachment jIssueAttachment = new JIssueAttachment();
                            try
                            {
                                jIssueAttachment.getIssueAttachmentFromJsonString(jsonData["fields"]["attachment"][n].ToJson());
                                if (jIssueAttachment != null)
                                {
                                    this.attachments.Add(jIssueAttachment);
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
                try
                {
                    if (jsonData["changelog"]["histories"].Count > 0)
                    {
                        for (int num = 0; num < jsonData["changelog"]["histories"].Count; num++)
                        {
                            JIssueChangelog jIssueChangelog = new JIssueChangelog();
                            try
                            {
                                jIssueChangelog.getChangeLogFromJsonString(jsonData["changelog"]["histories"][num].ToJson());
                                if (jIssueChangelog != null)
                                {
                                    this.changelogHistories.Add(jIssueChangelog);
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
            }
            catch (Exception message)
            {
                Debug.Log(message);
            }
            return this;
        }
    }
}
