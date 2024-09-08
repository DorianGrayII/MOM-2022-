namespace UnrealByte.EasyJira
{
    using LitJson;
    using System;

    public class JErrorHandler
    {
        public static string ReadError(string errorCode, string text, string errorText)
        {
            string str = "";
            try
            {
                JsonData data = JsonMapper.ToObject(text);
                if (data["errorMessages"].Count > 0)
                {
                    str = data["errorMessages"][0].ToString();
                }
            }
            catch (Exception)
            {
                str = !errorCode.Equals("401") ? (!errorCode.Equals("404") ? errorText : "Host not found, check de base URL.") : "Please check the Jira user and password.";
            }
            return str;
        }
    }
}

