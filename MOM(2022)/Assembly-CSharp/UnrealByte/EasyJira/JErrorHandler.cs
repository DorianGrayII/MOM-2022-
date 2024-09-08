using System;
using LitJson;

namespace UnrealByte.EasyJira
{
    public class JErrorHandler
    {
        public static string ReadError(string errorCode, string text, string errorText)
        {
            string result = "";
            try
            {
                JsonData jsonData = JsonMapper.ToObject(text);
                if (jsonData["errorMessages"].Count > 0)
                {
                    result = jsonData["errorMessages"][0].ToString();
                }
            }
            catch (Exception)
            {
                result = (errorCode.Equals("401") ? "Please check the Jira user and password." : ((!errorCode.Equals("404")) ? errorText : "Host not found, check de base URL."));
            }
            return result;
        }
    }
}
