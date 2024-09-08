namespace MOM.Adventures
{
    using System;
    using System.Collections.Generic;

    public class NodeRelay : BaseNode
    {
        public override void InitializeOutputs()
        {
            base.outputs = new List<AdvOutput>();
            base.AddOutput(null);
        }

        public override void UpdateVisuals(EditorNode editorNode)
        {
            editorNode.UpdateOutputs();
        }
    }
}

