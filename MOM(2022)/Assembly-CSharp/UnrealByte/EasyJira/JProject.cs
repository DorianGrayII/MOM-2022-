namespace UnrealByte.EasyJira
{
    using System;

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

