using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using DBDef;
using MHUtils;
using MHUtils.UI;

namespace MOM.Adventures
{
    public class NodeBattle : BaseNode
    {
        public enum RESULT
        {
            Win = 0,
            Surrender = 1,
            Lost = 2
        }

        [XmlAttribute]
        public string playerInvolvedList;

        [XmlAttribute]
        [DefaultValue(null)]
        public string scriptName;

        [XmlAttribute]
        [DefaultValue(null)]
        public string scriptStringParameter;

        [XmlAttribute]
        [DefaultValue(null)]
        public string opponentGroupName;

        [XmlAttribute]
        [DefaultValue(null)]
        public string listA;

        [XmlAttribute]
        [DefaultValue(0)]
        public int level;

        [XmlAttribute]
        [DefaultValue(false)]
        public bool isScalable;

        public override void InitializeOutputs()
        {
            base.outputs = new List<AdvOutput>();
            base.AddOutput(RESULT.Win.ToString());
            base.AddOutput(RESULT.Surrender.ToString());
            base.AddOutput(RESULT.Lost.ToString());
        }

        public override void UpdateVisuals(EditorNode editorNode)
        {
            List<string> options = new List<string>(editorNode.GetAvaliableLists().Keys);
            List<string> metodsNamesOfType = ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.EditorBattleNode);
            HashSet<string> groups = new HashSet<string>();
            groups.Add("None");
            DataBase.GetType<global::DBDef.Group>().ForEach(delegate(global::DBDef.Group o)
            {
                groups.Add(o.dbName);
            });
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownOpponent", "opponentGroupName", groups.ToList());
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownParameter", "scriptName", metodsNamesOfType);
            UIComponentFill.LinkInputField<string>(editorNode.gameObject, this, "InputParameter", "scriptStringParameter");
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownList", "listA", options, delegate
            {
                this.UpdateVisuals(editorNode);
            });
            UIComponentFill.LinkToggle(editorNode.gameObject, this, "TogglePowerIsScalable", "isScalable");
            UIComponentFill.LinkInputField<int>(editorNode.gameObject, this, "InputLevel", "level");
            editorNode.UpdateOutputs();
        }

        public override bool RequiresOutputText()
        {
            return true;
        }
    }
}
