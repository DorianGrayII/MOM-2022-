using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace MOM.Adventures
{
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
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Module));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, this);
                memoryStream.Position = 0L;
                Module obj = xmlSerializer.Deserialize(memoryStream) as Module;
                EventEditorModules.GenerateUniqueID(obj);
                return obj;
            }
        }

        public int Test(bool showPopupIfOk = true)
        {
            int num = 0;
            string text = null;
            if (this.adventures == null || this.adventures.Count == 0)
            {
                text = "[EDITOR] Module " + this.name + " lacks events!";
                Debug.LogWarning(text);
                PopupGeneral.OpenPopup(null, "Finished", text, "UI_OKAY");
                num++;
            }
            else
            {
                foreach (Adventure adventure in this.adventures)
                {
                    num += adventure.Test(this);
                }
            }
            if (num > 0)
            {
                text = "[EDITOR] Detected " + num + " error(s) in the module " + this.name + "!\nCheck C:\\Users\\[USER_NAME]\\AppData\\LocalLow\\MuHa Games\\MoM\\GameLog.txt for details";
            }
            else
            {
                if (!showPopupIfOk)
                {
                    return num;
                }
                text = "[EDITOR] Detected no errors in the module " + this.name;
            }
            Debug.LogWarning(text);
            PopupGeneral.OpenPopup(null, "Finished", text, "UI_OKAY");
            return num;
        }
    }
}
