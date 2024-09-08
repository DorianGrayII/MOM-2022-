using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using DBDef;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MOM.Adventures
{
    public class LogicEntry : AdvLogic
    {
        public enum LEntry
        {
            NONE = 0,
            LEEntryCity = 1,
            LEEntryEnchantment = 2,
            LEEntryHero = 3,
            LEEntryUnit = 4,
            LEEntryWizard = 5,
            LEEntryEquipment = 6,
            LEEntryGroup = 7
        }

        public enum PlayerOwner
        {
            ActivePlayer = 0,
            NonNeutralPlayer = 1,
            OpponentPlayers = 2,
            AnyPlayer = 3,
            NeutralPlayers = 4
        }

        public enum CityType
        {
            None = 0,
            Capitol = 1,
            All = 2,
            Outpost = 3,
            Hamlet = 4,
            Vilage = 5,
            Town = 6,
            Citi = 7
        }

        [XmlAttribute]
        public LEntry logicType;

        [XmlAttribute]
        [DefaultValue(null)]
        public string listName;

        [XmlAttribute]
        [DefaultValue(false)]
        public bool makeListPublic;

        [XmlAttribute]
        public PlayerOwner playerOwner;

        [XmlAttribute]
        [DefaultValue(CityType.None)]
        public CityType cityType;

        [XmlAttribute]
        [DefaultValue(null)]
        public TAG tagType;

        [XmlAttribute]
        [DefaultValue(null)]
        public PLANE planeType;

        [XmlAttribute]
        [DefaultValue(null)]
        public string scriptName;

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
            UIComponentFill.LinkDropdownEnum<PlayerOwner>(go, this, "DropdownSource", "playerOwner");
            List<string> options;
            switch (this.logicType)
            {
            case LEntry.LEEntryCity:
                UIComponentFill.LinkDropdownEnum<CityType>(go, this, "DropdownCapitol", "cityType");
                options = ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.EditorCityFilter);
                break;
            case LEntry.LEEntryEnchantment:
                options = ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.EditorEnchantmentFilter);
                break;
            case LEntry.LEEntryHero:
                options = ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.EditorHeroFilter);
                break;
            case LEntry.LEEntryUnit:
                options = ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.EditorUnitFilter);
                break;
            case LEntry.LEEntryWizard:
                options = ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.EditorWizardFilter);
                break;
            case LEntry.LEEntryEquipment:
                options = ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.EditorEquipmentFilter);
                break;
            case LEntry.LEEntryGroup:
                options = ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.EditorGroupFilter);
                break;
            default:
                options = new List<string>();
                break;
            }
            UIComponentFill.LinkDropdown(go, this, "DropdownSelect", "scriptName", options, delegate
            {
                this.PopulateGO(go, node);
            });
            UIComponentFill.LinkInputField<string>(go, this, "InputParameter", "scriptStringParameter");
            IEnumerable<ScriptParameters> metodParameterType = ScriptLibrary.GetMetodParameterType(GameObjectUtils.FindByNameGetComponent<DropDownFilters>(go, "DropdownSelect").GetSelection());
            HashSet<string> parameters = new HashSet<string>();
            bool flag = true;
            if (metodParameterType != null)
            {
                foreach (ScriptParameters item in metodParameterType)
                {
                    flag = true;
                    if (item.dropdownType == null)
                    {
                        continue;
                    }
                    if (item.dropdownType.IsSubclassOf(typeof(DBClass)))
                    {
                        DataBase.GetType(item.dropdownType).ForEach(delegate(DBClass o)
                        {
                            parameters.Add(o.dbName);
                        });
                    }
                    else if (item.dropdownType.IsEnum)
                    {
                        UIComponentFill.LinkDropdownEnumStringField(item.dropdownType, go, this, "DropdownParameter", "scriptTypeParameter");
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
            }
            List<string> options2 = new List<string>();
            UIComponentFill.LinkDropdown(go, this, "DropdownDebug", "debugScriptName", options2);
        }

        public override string GetScriptName()
        {
            return this.scriptName;
        }
    }
}
