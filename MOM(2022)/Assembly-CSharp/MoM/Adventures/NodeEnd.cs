namespace MOM.Adventures
{
    using MHUtils.UI;
    using MOM;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;

    public class NodeEnd : BaseNode
    {
        [XmlAttribute]
        public int navigateToEvent;
        [XmlAttribute, DefaultValue(false)]
        public bool destroyEvent;

        public override void InitializeOutputs()
        {
            base.outputs = null;
        }

        public override void UpdateVisuals(EditorNode editorNode)
        {
            List<Adventure> adventures = EventEditorModules.GetSelectedModule().adventures;
            if (adventures != null)
            {
                List<string> list = new List<string>(adventures.Count) { "None" };
                adventures.ForEach(o => list.Add(o.name));
                UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownNavigateToEvent", "NavigateToEvent", list, null, false);
                UIComponentFill.LinkToggle(editorNode.gameObject, editorNode.GetBaseNode(), "ToggleDestroyEvent", "destroyEvent", null);
            }
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

