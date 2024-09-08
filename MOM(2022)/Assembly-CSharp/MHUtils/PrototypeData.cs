namespace MHUtils
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml;

    [StructLayout(LayoutKind.Sequential)]
    public struct PrototypeData
    {
        public XmlElement data;
        public string dbName;
        public string name;
        public string abbreviation;
        public string comment;
    }
}

