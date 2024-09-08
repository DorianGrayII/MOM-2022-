using System;
using LitJson;

namespace UnrealByte.EasyJira
{
    public class JIssueAttachment
    {
        public string fileName = "";

        public string created = "";

        public string size = "";

        public string mimeType = "";

        public string content = "";

        public string authorDisplayName = "";

        public JIssueAttachment getIssueAttachmentFromJsonString(string JSONString)
        {
            JsonData jsonData = JsonMapper.ToObject(JSONString);
            try
            {
                try
                {
                    this.fileName = jsonData["filename"].ToString();
                }
                catch (Exception)
                {
                    this.fileName = "---";
                }
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
                    this.size = jsonData["size"].ToString();
                }
                catch (Exception)
                {
                    this.size = "---";
                }
                try
                {
                    this.mimeType = jsonData["mimeType"].ToString();
                }
                catch (Exception)
                {
                    this.mimeType = "---";
                }
                try
                {
                    this.content = jsonData["content"].ToString();
                }
                catch (Exception)
                {
                    this.content = "---";
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
