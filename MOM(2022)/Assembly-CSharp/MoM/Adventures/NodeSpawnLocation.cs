using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using DBDef;
using MHUtils;
using MHUtils.UI;

namespace MOM.Adventures
{
    public class NodeSpawnLocation : BaseNode
    {
        [XmlAttribute]
        [DefaultValue(null)]
        public string scriptTraderTypeParameter;

        [XmlAttribute]
        [DefaultValue(null)]
        public string scriptName;

        [XmlAttribute]
        [DefaultValue(null)]
        public string spawnName;

        [XmlAttribute]
        [DefaultValue(null)]
        public string anchorName;

        [XmlAttribute]
        [DefaultValue(false)]
        public bool destroyOwner;

        [XmlAttribute]
        [DefaultValue(null)]
        public string distance;

        [XmlAttribute]
        public int navigateToEvent;

        [XmlIgnore]
        public string NavigateToEvent
        {
            get
            {
                return EventEditorModules.GetSelectedModule().adventures?.Find((Adventure o) => o.uniqueID == this.navigateToEvent)?.name;
            }
            set
            {
                List<Adventure> adventures = EventEditorModules.GetSelectedModule().adventures;
                if (adventures != null)
                {
                    Adventure adventure = adventures.Find((Adventure o) => o.name == value);
                    if (adventure == null)
                    {
                        this.navigateToEvent = 0;
                    }
                    else
                    {
                        this.navigateToEvent = adventure.uniqueID;
                    }
                }
            }
        }

        public override void InitializeOutputs()
        {
            base.outputs = new List<AdvOutput>();
            base.AddOutput();
        }

        public override void UpdateVisuals(EditorNode editorNode)
        {
            List<string> spawnObjects = new List<string>();
            List<string> avaliableListsOf = editorNode.GetAvaliableListsOf();
            List<string> list = new List<string>();
            list.Add("None");
            list.AddRange(ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.EditorSpawnLocation));
            List<DBClass> type = DataBase.GetType(typeof(global::DBDef.Location));
            List<DBClass> type2 = DataBase.GetType(typeof(global::DBDef.Group));
            spawnObjects.Add("None");
            type.ForEach(delegate(DBClass o)
            {
                spawnObjects.Add(o.dbName);
            });
            type2.ForEach(delegate(DBClass o)
            {
                spawnObjects.Add(o.dbName);
            });
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownWhat", "spawnName", spawnObjects);
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownAnchor", "anchorName", avaliableListsOf);
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownScript", "scriptName", list);
            UIComponentFill.LinkInputField<string>(editorNode.gameObject, this, "InputDistance", "distance");
            UIComponentFill.LinkToggle(editorNode.gameObject, editorNode.GetBaseNode(), "ToggleDestroyOwner", "destroyOwner");
            List<Adventure> adventures = EventEditorModules.GetSelectedModule().adventures;
            if (adventures != null)
            {
                List<string> eventList = new List<string>(adventures.Count);
                eventList.Add("None");
                adventures.ForEach(delegate(Adventure o)
                {
                    eventList.Add(o.name);
                });
                UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownEvent", "NavigateToEvent", eventList);
            }
            editorNode.UpdateOutputs();
        }
    }
}
