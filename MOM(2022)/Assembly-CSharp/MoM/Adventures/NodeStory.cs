namespace MOM.Adventures
{
    using MHUtils.UI;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;

    public class NodeStory : BaseNode
    {
        [XmlAttribute, DefaultValue((string) null)]
        public string graphic;
        [XmlAttribute]
        public string story;

        public override void InitializeOutputs()
        {
            base.outputs = new List<AdvOutput>();
            base.AddOutput(null);
        }

        public override bool RequiresOutputText()
        {
            return true;
        }

        public override void UpdateVisuals(EditorNode editorNode)
        {
            UIComponentFill.LinkInputField<string>(editorNode.gameObject, editorNode.GetBaseNode(), "InputPhaseText", "story", null, null);
            UIComponentFill.LinkToggle(editorNode.gameObject, editorNode.GetBaseNode(), "ToggleTriggerOnce", "allowOnce", null);
            editorNode.UpdateGraphic();
            editorNode.UpdateOutputs();
        }
    }
}

