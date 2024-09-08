using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using MHUtils.UI;

namespace MOM.Adventures
{
    public class NodeTrade : BaseNode
    {
        public enum TradeCurrency
        {
            None = 0,
            Gold = 1,
            Mana = 2,
            Both = 3
        }

        [XmlAttribute]
        [DefaultValue(null)]
        public string playerWares;

        [XmlAttribute]
        [DefaultValue(null)]
        public string scriptTypeParameter;

        [XmlAttribute]
        [DefaultValue(null)]
        public string scriptStringParameter;

        [XmlAttribute]
        [DefaultValue(null)]
        public string traderWares;

        [XmlAttribute]
        [DefaultValue(null)]
        public string scriptTraderTypeParameter;

        [XmlAttribute]
        [DefaultValue(null)]
        public string scriptTraderStringParameter;

        [XmlAttribute]
        [DefaultValue(TradeCurrency.None)]
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
            new List<string>();
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownPlayersWares", "playerWares", avaliableListsOf);
            UIComponentFill.LinkDropdownEnum<TradeCurrency>(editorNode.gameObject, this, "DropdownPlayersWaresParameter", "tradeCurrency");
            UIComponentFill.LinkInputField<string>(editorNode.gameObject, this, "InputPlayersWaresParameter", "scriptStringParameter");
            List<string> options = new List<string>();
            List<string> options2 = new List<string>();
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownTradersWares", "traderWares", options);
            UIComponentFill.LinkDropdown(editorNode.gameObject, this, "DropdownTradersWaresParameter", "scriptTraderTypeParameter", options2);
            UIComponentFill.LinkInputField<string>(editorNode.gameObject, this, "InputTradersWaresParameter", "scriptTraderStringParameter");
            editorNode.UpdateOutputs();
        }
    }
}
