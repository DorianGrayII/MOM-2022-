namespace MOM.Adventures
{
    using MHUtils.UI;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;

    public class NodeStart : BaseNode
    {
        [XmlAttribute]
        public Adventure.AdventureTriggerType adventureStartType;
        [XmlAttribute]
        public Adventure.AdventurePositiveType adventurePositiveType;
        [XmlAttribute, DefaultValue((string) null)]
        public string graphic;
        [XmlAttribute]
        public int cooldown;
        [XmlAttribute]
        public int initialDelay;
        [XmlAttribute, DefaultValue(false)]
        public bool genericEvent;

        public override HashSet<BaseNode> GetInputs()
        {
            return null;
        }

        public override void InitializeOutputs()
        {
            base.outputs = new List<AdvOutput>();
            base.AddOutput(null);
        }

        public override void UpdateVisuals(EditorNode editorNode)
        {
            UIComponentFill.LinkDropdownEnum<Adventure.AdventureTriggerType>(editorNode.gameObject, this, "DropdownEventType", "adventureStartType", null);
            UIComponentFill.LinkDropdownEnum<Adventure.AdventurePositiveType>(editorNode.gameObject, this, "DropdownEventAlignment", "adventurePositiveType", null);
            UIComponentFill.LinkToggle(editorNode.gameObject, editorNode.GetBaseNode(), "ToggleTriggerOnce", "allowOnce", null);
            UIComponentFill.LinkToggle(editorNode.gameObject, editorNode.GetBaseNode(), "ToggleGenericEvent", "genericEvent", null);
            UIComponentFill.LinkInputField<int>(editorNode.gameObject, editorNode.GetBaseNode(), "InputDelay", "initialDelay", null, null);
            UIComponentFill.LinkInputField<int>(editorNode.gameObject, editorNode.GetBaseNode(), "InputCooldown", "cooldown", null, null);
            editorNode.UpdateGraphic();
            editorNode.UpdateOutputs();
        }
    }
}

