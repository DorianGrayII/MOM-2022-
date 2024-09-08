namespace UnrealByte.EasyJira
{
    using LitJson;
    using System;

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
            JsonData data = JsonMapper.ToObject(JSONString);
            try
            {
                try
                {
                    this.fileName = data["filename"].ToString();
                }
                catch (Exception)
                {
                    this.fileName = "---";
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
                    this.content = data["content"].ToString();
                }
                catch (Exception)
                {
                    this.content = "---";
                }
                goto TR_0003;
            TR_0009:
                try
                {
                    this.mimeType = data["mimeType"].ToString();
                }
                catch (Exception)
                {
                    this.mimeType = "---";
                }
                goto TR_0006;
            TR_000C:
                try
                {
                    this.size = data["size"].ToString();
                }
                catch (Exception)
                {
                    this.size = "---";
                }
                goto TR_0009;
            TR_000F:
                try
                {
                    this.created = DateTime.Parse(data["created"].ToString()).ToShortDateString();
                }
                catch (Exception)
                {
                    this.created = "---";
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

