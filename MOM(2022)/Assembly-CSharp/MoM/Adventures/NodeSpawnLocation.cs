namespace MOM.Adventures
{
    using DBDef;
    using MHUtils;
    using MHUtils.UI;
    using MOM;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;

    public class NodeSpawnLocation : BaseNode
    {
        [XmlAttribute, DefaultValue((string) null)]
        public string scriptTraderTypeParameter;
        [XmlAttribute, DefaultValue((string) null)]
        public string scriptName;
        [XmlAttribute, DefaultValue((string) null)]
        public string spawnName;
        [XmlAttribute, DefaultValue((string) null)]
        public string anchorName;
        [XmlAttribute, DefaultValue(false)]
        public bool destroyOwner;
        [XmlAttribute, DefaultValue((string) null)]
        public string distance;
        [XmlAttribute]
        public int navigateToEvent;

        public override void InitializeOutputs()
        {
            base.outputs = new List<AdvOutput>();
            base.AddOutput(null);
        }

        public override void UpdateVisuals(EditorNode editorNode)
        {
            List<string> spawnObjects = new List<string>();
            List<string> avaliableListsOf = editorNode.GetAvaliableListsOf();
            List<string> options = new List<string> { "None" };
            options.AddRange(ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.EditorSpawnLocation));
            List<DBClass> type = DataBase.GetType(typeof(DBDef.Location));
            spawnObjects.Add("None");
            type.ForEach(o => spawnObjects.Add(o.dbName));
            DataBase.GetType(typeof(DBDef.Group)).ForEach(o => spawnObjects.Add(o.dbName));
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownWhat", "spawnName", spawnObjects, null, false);
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownAnchor", "anchorName", avaliableListsOf, null, false);
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownScript", "scriptName", options, null, false);
            UIComponentFill.LinkInputField<string>(editorNode.gameObject, this, "InputDistance", "distance", null, null);
            UIComponentFill.LinkToggle(editorNode.gameObject, editorNode.GetBaseNode(), "ToggleDestroyOwner", "destroyOwner", null);
            List<Adventure> adventures = EventEditorModules.GetSelectedModule().adventures;
            if (adventures != null)
            {
                List<string> eventList = new List<string>(adventures.Count) { "None" };
                adventures.ForEach(o => eventList.Add(o.name));
                UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownEvent", "NavigateToEvent", eventList, null, false);
            }
            editorNode.UpdateOutputs();
        }

        [XmlIgnore]
        public string NavigateToEvent
        {
            get
            {
                List<Adventure> adventures = EventEditorModules.GetSelectedModule().adventures;
                if (adventures == null)
                {
                    return null;
                }
                Adventure adventure = adventures.Find(o => o.uniqueID == this.navigateToEvent);
                return ((adventure != null) ? adventure.name : null);
            }
            set
            {
                List<Adventure> adventures = EventEditorModules.GetSelectedModule().adventures;
                if (adventures != null)
                {
                    Adventure adventure = adventures.Find(o => o.name == value);
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
    }
}

