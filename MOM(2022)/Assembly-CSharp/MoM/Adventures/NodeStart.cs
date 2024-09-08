using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using MHUtils.UI;

namespace MOM.Adventures
{
    public class NodeStart : BaseNode
    {
        [XmlAttribute]
        public Adventure.AdventureTriggerType adventureStartType;

        [XmlAttribute]
        public Adventure.AdventurePositiveType adventurePositiveType;

        [XmlAttribute]
        [DefaultValue(null)]
        public string graphic;

        [XmlAttribute]
        public int cooldown;

        [XmlAttribute]
        public int initialDelay;

        [XmlAttribute]
        [DefaultValue(false)]
        public bool genericEvent;

        public override HashSet<BaseNode> GetInputs()
        {
            return null;
        }

        public override void InitializeOutputs()
        {
            base.outputs = new List<AdvOutput>();
            base.AddOutput();
        }

        public override void UpdateVisuals(EditorNode editorNode)
        {
            UIComponentFill.LinkDropdownEnum<Adventure.AdventureTriggerType>(editorNode.gameObject, this, "DropdownEventType", "adventureStartType");
            UIComponentFill.LinkDropdownEnum<Adventure.AdventurePositiveType>(editorNode.gameObject, this, "DropdownEventAlignment", "adventurePositiveType");
            UIComponentFill.LinkToggle(editorNode.gameObject, editorNode.GetBaseNode(), "ToggleTriggerOnce", "allowOnce");
            UIComponentFill.LinkToggle(editorNode.gameObject, editorNode.GetBaseNode(), "ToggleGenericEvent", "genericEvent");
            UIComponentFill.LinkInputField<int>(editorNode.gameObject, editorNode.GetBaseNode(), "InputDelay", "initialDelay");
            UIComponentFill.LinkInputField<int>(editorNode.gameObject, editorNode.GetBaseNode(), "InputCooldown", "cooldown");
            editorNode.UpdateGraphic();
            editorNode.UpdateOutputs();
        }
    }
}
