namespace UnrealByte.EasyJira
{
    using LitJson;
    using System;
    using UnityEngine;

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
            JsonData data = JsonMapper.ToObject(JSONString);
            try
            {
                try
                {
                    this.created = DateTime.Parse(data["created"].ToString()).ToShortDateString();
                }
                catch (Exception)
                {
                    this.created = "---";
                }
                goto TR_0015;
            TR_0003:
                try
                {
                    this.authorDisplayName = data["author"]["displayName"].ToString();
                }
                catch (Exception)
                {
                    this.authorDisplayName = "---";
                }
                goto TR_0000;
            TR_0006:
                try
                {
                    this.authorAvatar = data["author"]["avatarUrls"]["48x48"].ToString();
                }
                catch (Exception)
                {
                    this.authorAvatar = "---";
                }
                goto TR_0003;
            TR_0009:
                try
                {
                    this.authorEmail = data["author"]["emailAddress"].ToString();
                }
                catch (Exception)
                {
                    this.authorEmail = "---";
                }
                goto TR_0006;
            TR_000C:
                try
                {
                    this.authorName = data["author"]["name"].ToString();
                }
                catch (Exception)
                {
                    this.authorName = "---";
                }
                goto TR_0009;
            TR_000F:
                try
                {
                    this.toValue = data["items"][0]["toString"].ToString();
                }
                catch (Exception)
                {
                    this.toValue = "---";
                }
                goto TR_000C;
            TR_0012:
                try
                {
                    this.fromValue = data["items"][0]["fromString"].ToString();
                }
                catch (Exception)
                {
                    this.fromValue = "---";
                }
                goto TR_000F;
            TR_0015:
                try
                {
                    this.field = data["items"][0]["field"].ToString();
                }
                catch (Exception)
                {
                    this.field = "---";
                }
                goto TR_0012;
            }
            catch (Exception)
            {
            }
        TR_0000:
            return this;
        }
    }
}

