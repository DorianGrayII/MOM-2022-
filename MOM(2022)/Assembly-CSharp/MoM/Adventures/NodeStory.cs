using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using MHUtils.UI;

namespace MOM.Adventures
{
    public class NodeStory : BaseNode
    {
        [XmlAttribute]
        [DefaultValue(null)]
        public string graphic;

        [XmlAttribute]
        public string story;

        public override void InitializeOutputs()
        {
            base.outputs = new List<AdvOutput>();
            base.AddOutput();
        }

        public override void UpdateVisuals(EditorNode editorNode)
        {
            UIComponentFill.LinkInputField<string>(editorNode.gameObject, editorNode.GetBaseNode(), "InputPhaseText", "story");
            UIComponentFill.LinkToggle(editorNode.gameObject, editorNode.GetBaseNode(), "ToggleTriggerOnce", "allowOnce");
            editorNode.UpdateGraphic();
            editorNode.UpdateOutputs();
        }

        public override bool RequiresOutputText()
        {
            return true;
        }
    }
}
