namespace MOM.Adventures
{
    using MOM;
    using System;
    using System.Collections.Generic;

    public class NodeLocationAward : BaseNode
    {
        public void ClaimAward(PlayerWizard wizard, Location location, IGroup iGroup, List<KeyValuePair<Unit, IGroup>> heroes)
        {
            object[] parameters = new object[5];
            parameters[0] = wizard;
            parameters[1] = location;
            parameters[2] = iGroup;
            parameters[3] = heroes;
            ScriptLibrary.Call("ClaimAward", parameters);
        }

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

