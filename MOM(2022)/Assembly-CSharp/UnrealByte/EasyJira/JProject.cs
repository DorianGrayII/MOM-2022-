using System;

namespace UnrealByte.EasyJira
{
    [Serializable]
    public class JProject
    {
        public string key = "";

        public JProject(string key)
        {
            this.key = key;
        }
    }
}
