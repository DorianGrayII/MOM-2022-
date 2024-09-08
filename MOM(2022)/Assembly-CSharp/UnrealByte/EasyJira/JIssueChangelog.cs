using System;
using LitJson;
using UnityEngine;

namespace UnrealByte.EasyJira
{
    public class JIssueChangelog
    {
        public string created = "";

        public string field = "";

        public string fromValue = "";

        public string toValue = "";

        public string authorName = "";

        public string authorEmail = "";

        public string authorDisplayName = "";

        public string authorAvatar = "";

        public Texture2D authorAvatarTexture = new Texture2D(30, 30);

        public JIssueChangelog getChangeLogFromJsonString(string JSONString)
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
                    this.field = jsonData["items"][0]["field"].ToString();
                }
                catch (Exception)
                {
                    this.field = "---";
                }
                try
                {
                    this.fromValue = jsonData["items"][0]["fromString"].ToString();
                }
                catch (Exception)
                {
                    this.fromValue = "---";
                }
                try
                {
                    this.toValue = jsonData["items"][0]["toString"].ToString();
                }
                catch (Exception)
                {
                    this.toValue = "---";
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
