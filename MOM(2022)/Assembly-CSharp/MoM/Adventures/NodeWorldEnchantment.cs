namespace MOM.Adventures
{
    using DBDef;
    using MHUtils;
    using MHUtils.UI;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;

    public class NodeWorldEnchantment : BaseNode
    {
        [XmlAttribute, DefaultValue((string) null)]
        public string enchantmentName;
        [XmlAttribute, DefaultValue((string) null)]
        public string scriptTypeParameter;
        [XmlAttribute, DefaultValue((string) null)]
        public string scriptStringParameter;
        [XmlAttribute, DefaultValue((string) null)]
        public string scriptStringDuration;
        [XmlAttribute, DefaultValue((string) null)]
        public string targetName;

        public override void InitializeOutputs()
        {
            base.outputs = new List<AdvOutput>();
            base.AddOutput(null);
        }

        public override void UpdateVisuals(EditorNode editorNode)
        {
            List<string> worldEnchantments = new List<string>();
            DataBase.GetType(typeof(Enchantment)).ForEach(o => worldEnchantments.Add(o.dbName));
            List<string> avaliableListsOf = editorNode.GetAvaliableListsOf();
            avaliableListsOf.AddRange(Enum.GetNames(typeof(WizardCriteria)));
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownWizard", "targetName", avaliableListsOf, o => this.UpdateVisuals(editorNode), false);
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownParameter", "enchantmentName", worldEnchantments, null, false);
            UIComponentFill.LinkInputField<string>(editorNode.gameObject, this, "InputParameter", "scriptStringParameter", null, null);
            UIComponentFill.LinkInputField<string>(editorNode.gameObject, this, "InputDuration", "scriptStringDuration", null, null);
            editorNode.UpdateOutputs();
        }

        public enum WizardCriteria
        {
            AllWizzards,
            CurrentWizzard
        }
    }
}

