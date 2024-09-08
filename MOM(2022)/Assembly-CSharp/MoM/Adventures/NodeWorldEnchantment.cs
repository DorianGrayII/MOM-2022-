using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using DBDef;
using MHUtils;
using MHUtils.UI;

namespace MOM.Adventures
{
    public class NodeWorldEnchantment : BaseNode
    {
        public enum WizardCriteria
        {
            AllWizzards = 0,
            CurrentWizzard = 1
        }

        [XmlAttribute]
        [DefaultValue(null)]
        public string enchantmentName;

        [XmlAttribute]
        [DefaultValue(null)]
        public string scriptTypeParameter;

        [XmlAttribute]
        [DefaultValue(null)]
        public string scriptStringParameter;

        [XmlAttribute]
        [DefaultValue(null)]
        public string scriptStringDuration;

        [XmlAttribute]
        [DefaultValue(null)]
        public string targetName;

        public override void InitializeOutputs()
        {
            base.outputs = new List<AdvOutput>();
            base.AddOutput();
        }

        public override void UpdateVisuals(EditorNode editorNode)
        {
            List<string> worldEnchantments = new List<string>();
            DataBase.GetType(typeof(Enchantment)).ForEach(delegate(DBClass o)
            {
                worldEnchantments.Add(o.dbName);
            });
            List<string> avaliableListsOf = editorNode.GetAvaliableListsOf();
            avaliableListsOf.AddRange(Enum.GetNames(typeof(WizardCriteria)));
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownWizard", "targetName", avaliableListsOf, delegate
            {
                this.UpdateVisuals(editorNode);
            });
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownParameter", "enchantmentName", worldEnchantments);
            UIComponentFill.LinkInputField<string>(editorNode.gameObject, this, "InputParameter", "scriptStringParameter");
            UIComponentFill.LinkInputField<string>(editorNode.gameObject, this, "InputDuration", "scriptStringDuration");
            editorNode.UpdateOutputs();
        }
    }
}
