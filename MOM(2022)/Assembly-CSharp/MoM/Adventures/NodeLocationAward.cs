using System.Collections.Generic;

namespace MOM.Adventures
{
    public class NodeLocationAward : BaseNode
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

        public void ClaimAward(PlayerWizard wizard, Location location, IGroup iGroup, List<KeyValuePair<Unit, IGroup>> heroes)
        {
            ScriptLibrary.Call("ClaimAward", wizard, location, iGroup, heroes, null);
        }
    }
}
