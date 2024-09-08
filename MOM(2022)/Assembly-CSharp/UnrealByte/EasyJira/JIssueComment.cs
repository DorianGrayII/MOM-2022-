namespace UnrealByte.EasyJira
{
    using LitJson;
    using System;
    using UnityEngine;

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
                goto TR_000F;
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
                    this.body = data["body"].ToString();
                }
                catch (Exception)
                {
                    this.body = "---";
                }
                goto TR_000C;
            }
            catch (Exception)
            {
            }
        TR_0000:
            return this;
        }
    }
}

