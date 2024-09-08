using System;
using LitJson;
using UnityEngine;

namespace UnrealByte.EasyJira
{
    public class JIssueComment
    {
        public string created = "";

        public string body = "";

        public string authorName = "";

        public string authorEmail = "";

        public string authorDisplayName = "";

        public string authorAvatar = "";

        public Texture2D authorAvatarTexture = new Texture2D(30, 30);

        public JIssueComment getIssueCommentFromJsonString(string JSONString)
        {
            JsonData jsonData = JsonMapper.ToObject(JSONString);
            try
            {
                try
                {
                    this.created = DateTime.Parse(jsonData["created"].ToString()).ToShortDateString();
                }
                catch (Exception)
                {
                    this.created = "---";
                }
                try
                {
                    this.body = jsonData["body"].ToString();
                }
                catch (Exception)
                {
                    this.body = "---";
                }
                try
                {
                    this.authorName = jsonData["author"]["name"].ToString();
                }
                catch (Exception)
                {
                    this.authorName = "---";
                }
                try
                {
                    this.authorEmail = jsonData["author"]["emailAddress"].ToString();
                }
                catch (Exception)
                {
                    this.authorEmail = "---";
                }
                try
                {
                    this.authorAvatar = jsonData["author"]["avatarUrls"]["48x48"].ToString();
                }
                catch (Exception)
                {
                    this.authorAvatar = "---";
                }
                try
                {
                    this.authorDisplayName = jsonData["author"]["displayName"].ToString();
                }
                catch (Exception)
                {
                    this.authorDisplayName = "---";
                }
            }
            catch (Exception)
            {
            }
            return this;
        }
    }
}
