using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using MHUtils.UI;

namespace MOM.Adventures
{
    public class NodeEnd : BaseNode
    {
        [XmlAttribute]
        public int navigateToEvent;

        [XmlAttribute]
        [DefaultValue(false)]
        public bool destroyEvent;

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
            base.outputs = null;
        }

        public override void UpdateVisuals(EditorNode editorNode)
        {
            List<Adventure> adventures = EventEditorModules.GetSelectedModule().adventures;
            if (adventures != null)
            {
                List<string> list = new List<string>(adventures.Count);
                list.Add("None");
                adventures.ForEach(delegate(Adventure o)
                {
                    list.Add(o.name);
                });
                UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownNavigateToEvent", "NavigateToEvent", list);
                UIComponentFill.LinkToggle(editorNode.gameObject, editorNode.GetBaseNode(), "ToggleDestroyEvent", "destroyEvent");
            }
        }
    }
}
