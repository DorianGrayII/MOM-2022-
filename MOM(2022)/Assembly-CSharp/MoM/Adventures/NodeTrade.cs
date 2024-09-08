namespace MOM.Adventures
{
    using MHUtils.UI;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;

    public class NodeTrade : BaseNode
    {
        [XmlAttribute, DefaultValue((string) null)]
        public string playerWares;
        [XmlAttribute, DefaultValue((string) null)]
        public string scriptTypeParameter;
        [XmlAttribute, DefaultValue((string) null)]
        public string scriptStringParameter;
        [XmlAttribute, DefaultValue((string) null)]
        public string traderWares;
        [XmlAttribute, DefaultValue((string) null)]
        public string scriptTraderTypeParameter;
        [XmlAttribute, DefaultValue((string) null)]
        public string scriptTraderStringParameter;
        [XmlAttribute, DefaultValue(0)]
        public TradeCurrency tradeCurrency;

        public override void InitializeOutputs()
        {
            base.outputs = new List<AdvOutput>();
            base.AddOutput("Accepted");
            base.AddOutput("Canceled");
        }

        public override void UpdateVisuals(EditorNode editorNode)
        {
            List<string> avaliableListsOf = editorNode.GetAvaliableListsOf();
            List<string> list1 = new List<string>();
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownPlayersWares", "playerWares", avaliableListsOf, null, false);
            UIComponentFill.LinkDropdownEnum<TradeCurrency>(editorNode.gameObject, this, "DropdownPlayersWaresParameter", "tradeCurrency", null);
            UIComponentFill.LinkInputField<string>(editorNode.gameObject, this, "InputPlayersWaresParameter", "scriptStringParameter", null, null);
            List<string> options = new List<string>();
            List<string> list3 = new List<string>();
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownTradersWares", "traderWares", options, null, false);
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownTradersWaresParameter", "scriptTraderTypeParameter", list3, null, false);
            UIComponentFill.LinkInputField<string>(editorNode.gameObject, this, "InputTradersWaresParameter", "scriptTraderStringParameter", null, null);
            editorNode.UpdateOutputs();
        }

        public enum TradeCurrency
        {
            None,
            Gold,
            Mana,
            Both
        }
    }
}

