using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using DBDef;
using MHUtils;
using MHUtils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MOM.Adventures
{
    public class LogicProcessing : AdvLogic
    {
        public enum LProcessing
        {
            LEProcessingAdd = 0,
            LEProcessingFilter = 1,
            LEProcessingMultiply = 2,
            LEProcessingSubtract = 3
        }

        [XmlAttribute]
        public LProcessing logicType;

        [XmlAttribute]
        [DefaultValue(null)]
        public string listName;

        [XmlAttribute]
        [DefaultValue(null)]
        public string scriptName;

        [XmlAttribute]
        [DefaultValue(false)]
        public bool makeListPublic;

        [XmlAttribute]
        public LogicEntry.LEntry processType;

        [XmlAttribute]
        [DefaultValue(null)]
        public string listA;

        [XmlAttribute]
        [DefaultValue(null)]
        public string listB;

        [XmlAttribute]
        [DefaultValue(null)]
        public string scriptTypeParameter;

        [XmlAttribute]
        [DefaultValue(null)]
        public string scriptStringParameter;

        [XmlAttribute]
        [DefaultValue(null)]
        public string debugScriptName;

        public override string GetNodeName()
        {
            return this.logicType.ToString();
        }

        public override void PopulateGO(GameObject go, EditorNode node)
        {
            Button button = GameObjectUtils.FindByNameGetComponent<Button>(go, "ButtonDelete");
            Button button2 = GameObjectUtils.FindByNameGetComponent<Button>(go, "ButtonArrowUp");
            Button button3 = GameObjectUtils.FindByNameGetComponent<Button>(go, "ButtonArrowDown");
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate
            {
                MHEventSystem.TriggerEvent<AdvLogic>(this, "Delete");
            });
            button2.onClick.RemoveAllListeners();
            button2.onClick.AddListener(delegate
            {
                MHEventSystem.TriggerEvent<AdvLogic>(this, "Up");
            });
            button3.onClick.RemoveAllListeners();
            button3.onClick.AddListener(delegate
            {
                MHEventSystem.TriggerEvent<AdvLogic>(this, "Down");
            });
            UIComponentFill.LinkInputField<string>(go, this, "InputVariable", "listName", null, base.UpdateLogicBlocks);
            UIComponentFill.LinkToggle(go, this, "ToggleMakePublic", "makeListPublic", base.UpdateLogicBlocks);
            GameObjectUtils.FindByNameGetComponent<Toggle>(go, "ToggleMakePublic").onValueChanged.AddListener(delegate
            {
                MHEventSystem.TriggerEvent<EditorNode>(node, "Redraw");
            });
            Dictionary<string, LogicEntry.LEntry> lists = node.GetAvaliableLists();
            List<string> options = new List<string>(lists.Keys);
            Callback updateListB = delegate
            {
                if (this.logicType != LProcessing.LEProcessingFilter)
                {
                    LogicEntry.LEntry lEntry = LogicEntry.LEntry.LEEntryCity;
                    bool flag2 = false;
                    if (this.listA != null && lists.ContainsKey(this.listA))
                    {
                        lEntry = lists[this.listA];
                        flag2 = true;
                    }
                    List<string> list = new List<string>(lists.Keys.Count);
                    foreach (KeyValuePair<string, LogicEntry.LEntry> item in lists)
                    {
                        if (!flag2 || item.Value == lEntry)
                        {
                            list.Add(item.Key);
                        }
                    }
                    UIComponentFill.LinkDropdown(go, this, "DropdownListB", "listB", list);
                }
            };
            UIComponentFill.LinkDropdown(go, this, "DropdownListA", "listA", options, delegate(object o)
            {
                updateListB(o);
            });
            updateListB(null);
            if (this.logicType == LProcessing.LEProcessingFilter)
            {
                List<string> metodsNamesOfType = ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.EditorFilterListProcessing);
                UIComponentFill.LinkDropdown(go, this, "DropdownRule", "scriptName", metodsNamesOfType, delegate
                {
                    this.PopulateGO(go, node);
                });
                UIComponentFill.LinkInputField<string>(go, this, "InputParameter", "scriptStringParameter");
                IEnumerable<ScriptParameters> metodParameterType = ScriptLibrary.GetMetodParameterType(GameObjectUtils.FindByNameGetComponent<DropDownFilters>(go, "DropdownRule").GetSelection());
                HashSet<string> parameters = new HashSet<string>();
                bool flag = true;
                if (metodParameterType != null)
                {
                    foreach (ScriptParameters item2 in metodParameterType)
                    {
                        flag = true;
                        if (item2.dropdownType == null)
                        {
                            if (item2.dropdownListType == null)
                            {
                                continue;
                            }
                            foreach (KeyValuePair<string, LogicEntry.LEntry> item3 in lists)
                            {
                                if (item3.Value.Equals(item2.dropdownListType))
                                {
                                    parameters.Add(item3.Key);
                                }
                            }
                        }
                        else if (item2.dropdownType.IsSubclassOf(typeof(DBClass)))
                        {
                            DataBase.GetType(item2.dropdownType).ForEach(delegate(DBClass o)
                            {
                                parameters.Add(o.dbName);
                            });
                        }
                        else if (item2.dropdownType.IsEnum)
                        {
                            UIComponentFill.LinkDropdownEnumStringField(item2.dropdownType, go, this, "DropdownParameter", "scriptTypeParameter");
                            flag = false;
                        }
                    }
                }
                if (flag)
                {
                    if (parameters.Count == 0)
                    {
                        parameters.Add("None");
                    }
                    UIComponentFill.LinkDropdown(go, this, "DropdownParameter", "scriptTypeParameter", parameters.ToList());
                    UIComponentFill.LinkInputField<string>(go, this, "InputParameter", "scriptStringParameter", null, base.UpdateLogicBlocks);
                }
            }
            List<string> options2 = new List<string>();
            UIComponentFill.LinkDropdown(go, this, "DropdownDebug", "debugScriptName", options2);
        }

        public override string GetScriptName()
        {
            switch (this.logicType)
            {
            case LProcessing.LEProcessingFilter:
                return this.scriptName;
            case LProcessing.LEProcessingAdd:
                return "FLP_AandB";
            case LProcessing.LEProcessingMultiply:
                return "FLP_AorB";
            case LProcessing.LEProcessingSubtract:
                return "FLP_AminusB";
            default:
                return null;
            }
        }
    }
}
