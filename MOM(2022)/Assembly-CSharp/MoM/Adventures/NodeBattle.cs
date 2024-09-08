namespace MOM.Adventures
{
    using DBDef;
    using MHUtils;
    using MHUtils.UI;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;

    public class NodeBattle : BaseNode
    {
        [XmlAttribute]
        public string playerInvolvedList;
        [XmlAttribute, DefaultValue((string) null)]
        public string scriptName;
        [XmlAttribute, DefaultValue((string) null)]
        public string scriptStringParameter;
        [XmlAttribute, DefaultValue((string) null)]
        public string opponentGroupName;
        [XmlAttribute, DefaultValue((string) null)]
        public string listA;
        [XmlAttribute, DefaultValue(0)]
        public int level;
        [XmlAttribute, DefaultValue(false)]
        public bool isScalable;

        public override void InitializeOutputs()
        {
            base.outputs = new List<AdvOutput>();
            base.AddOutput(0.ToString());
            base.AddOutput(1.ToString());
            base.AddOutput(2.ToString());
        }

        public override bool RequiresOutputText()
        {
            return true;
        }

        public override void UpdateVisuals(EditorNode editorNode)
        {
            List<string> options = new List<string>(editorNode.GetAvaliableLists().Keys);
            List<string> metodsNamesOfType = ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.EditorBattleNode);
            HashSet<string> groups = new HashSet<string> { "None" };
            DataBase.GetType<Group>().ForEach(o => groups.Add(o.dbName));
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownOpponent", "opponentGroupName", Enumerable.ToList<string>(groups), null, false);
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownParameter", "scriptName", metodsNamesOfType, null, false);
            UIComponentFill.LinkInputField<string>(editorNode.gameObject, this, "InputParameter", "scriptStringParameter", null, null);
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownList", "listA", options, o => this.UpdateVisuals(editorNode), false);
            UIComponentFill.LinkToggle(editorNode.gameObject, this, "TogglePowerIsScalable", "isScalable", null);
            UIComponentFill.LinkInputField<int>(editorNode.gameObject, this, "InputLevel", "level", null, null);
            editorNode.UpdateOutputs();
        }

        public enum RESULT
        {
            Win,
            Surrender,
            Lost
        }
    }
}

