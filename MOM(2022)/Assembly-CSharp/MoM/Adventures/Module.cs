namespace MOM.Adventures
{
    using MOM;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Xml.Serialization;
    using UnityEngine;

    public class Module
    {
        [XmlAttribute]
        public bool isAllowed;
        [XmlAttribute]
        public int uniqueID;
        [XmlAttribute]
        public int nextAdventureID;
        [XmlAttribute]
        public string name;
        [XmlElement]
        public List<Adventure> adventures;

        public Module Clone()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Module));
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize((Stream) stream, this);
                stream.Position = 0L;
                Module m = serializer.Deserialize(stream) as Module;
                EventEditorModules.GenerateUniqueID(m);
                return m;
            }
        }

        public int Test(bool showPopupIfOk)
        {
            int num = 0;
            string message = null;
            if ((this.adventures == null) || (this.adventures.Count == 0))
            {
                message = "[EDITOR] Module " + this.name + " lacks events!";
                Debug.LogWarning(message);
                PopupGeneral.OpenPopup(null, "Finished", message, "UI_OKAY", null, null, null, null, null, null);
                num++;
            }
            else
            {
                foreach (Adventure adventure in this.adventures)
                {
                    num += adventure.Test(this);
                }
            }
            if (num <= 0)
            {
                if (!showPopupIfOk)
                {
                    return num;
                }
                message = "[EDITOR] Detected no errors in the module " + this.name;
            }
            else
            {
                string[] textArray1 = new string[] { "[EDITOR] Detected ", num.ToString(), " error(s) in the module ", this.name, "!\nCheck C:\\Users\\[USER_NAME]\\AppData\\LocalLow\\MuHa Games\\MoM\\GameLog.txt for details" };
                message = string.Concat(textArray1);
            }
            Debug.LogWarning(message);
            PopupGeneral.OpenPopup(null, "Finished", message, "UI_OKAY", null, null, null, null, null, null);
            return num;
        }
    }
}

