using System.Collections.Generic;

namespace MOM.Adventures
{
    public class NodeRelay : BaseNode
    {
        public override void InitializeOutputs()
        {
            base.outputs = new List<AdvOutput>();
            base.AddOutput();
        }

        public override void UpdateVisuals(EditorNode editorNode)
        {
            editorNode.UpdateOutputs();
        }
    }
}
