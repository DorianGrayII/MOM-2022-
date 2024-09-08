namespace MHUtils
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml;

    [StructLayout(LayoutKind.Sequential)]
    public struct DatabaseData
    {
        public XmlElement data;
        public string dbName;
    }
}

