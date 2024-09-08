// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.Adventures.LogicRequirement
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using DBDef;
using MHUtils;
using MHUtils.UI;
using MOM;
using MOM.Adventures;
using UnityEngine;
using UnityEngine.UI;

public class LogicRequirement : AdvLogic
{
    public enum LRequirement
    {
        LERequirementChance = 0,
        LERequirementCheckSharedTag = 1,
        LERequirementCityCriteria = 2,
        LERequirementFame = 3,
        LERequirementFamiliar = 4,
        LERequirementFood = 5,
        LERequirementGold = 6,
        LERequirementHeroCriteria = 7,
        LERequirementListSize1 = 8,
        LERequirementListSize2 = 9,
        LERequirementMagic = 10,
        LERequirementMana = 11,
        LERequirementTurn = 12,
        LERequirementWizardCriteria = 13,
        LERequirementOtherCriteria = 14
    }

    public enum LogicGroupHint
    {
        None = 0,
        G1 = 1,
        G2 = 2,
        G3 = 3,
        G4 = 4,
        G5 = 5,
        G6 = 6,
        G7 = 7,
        G8 = 8
    }

    [XmlAttribute]
    public LRequirement logicType;

    [XmlAttribute]
    public LogicEntry.PlayerOwner playerOwner;

    [XmlAttribute]
    public LogicUtils.Comparison logicComparison;

    [XmlAttribute]
    [DefaultValue(null)]
    public string listA;

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
    public string scriptStringValue;

    [XmlAttribute]
    [DefaultValue(false)]
    public bool lessThan;

    [XmlAttribute]
    [DefaultValue(false)]
    public bool equal;

    [XmlAttribute]
    [DefaultValue(false)]
    public bool moreThan;

    [XmlAttribute]
    [DefaultValue(LogicGroupHint.None)]
    public LogicGroupHint group;

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
        List<string> list = new List<string>();
        switch (this.logicType)
        {
        case LRequirement.LERequirementChance:
            UIComponentFill.LinkInputField<string>(go, this, "InputValue", "scriptStringValue");
            break;
        case LRequirement.LERequirementCheckSharedTag:
            UIComponentFill.LinkDropdownEnum<LogicEntry.PlayerOwner>(go, this, "DropdownSource", "playerOwner");
            list = ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.EditorSharedTagResult);
            UIComponentFill.LinkInputField<string>(go, this, "InputParameter", "scriptStringParameter");
            break;
        case LRequirement.LERequirementCityCriteria:
        {
            List<string> avaliableListsOf4 = node.GetAvaliableListsOf(LogicEntry.LEntry.LEEntryCity);
            UIComponentFill.LinkDropdown(go, this, "DropdownListA", "listA", avaliableListsOf4);
            list = ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.EditorCityResult);
            UIComponentFill.LinkInputField<string>(go, this, "InputParameter", "scriptStringParameter");
            break;
        }
        case LRequirement.LERequirementFame:
            UIComponentFill.LinkDropdownEnum<LogicEntry.PlayerOwner>(go, this, "DropdownSource", "playerOwner");
            UIComponentFill.LinkToggle(go, this, "ToggleLess", "lessThan");
            UIComponentFill.LinkToggle(go, this, "ToggleEqual", "equal");
            UIComponentFill.LinkToggle(go, this, "ToggleMore", "moreThan");
            UIComponentFill.LinkInputField<string>(go, this, "InputParameter", "scriptStringParameter");
            break;
        case LRequirement.LERequirementFamiliar:
            UIComponentFill.LinkDropdownEnum<LogicEntry.PlayerOwner>(go, this, "DropdownSource", "playerOwner");
            list = ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.EditorFamiliarResult);
            UIComponentFill.LinkInputField<string>(go, this, "InputParameter", "scriptStringParameter");
            break;
        case LRequirement.LERequirementFood:
        case LRequirement.LERequirementGold:
        case LRequirement.LERequirementMana:
            UIComponentFill.LinkDropdownEnum<LogicEntry.PlayerOwner>(go, this, "DropdownSource", "playerOwner");
            UIComponentFill.LinkDropdownEnum<LogicUtils.Comparison>(go, this, "DropdownScript", "logicComparison");
            UIComponentFill.LinkInputField<string>(go, this, "InputParameter", "scriptStringParameter");
            break;
        case LRequirement.LERequirementMagic:
        {
            UIComponentFill.LinkDropdownEnum<LogicEntry.PlayerOwner>(go, this, "DropdownSource", "playerOwner");
            UIComponentFill.LinkDropdownEnum<LogicUtils.Comparison>(go, this, "DropdownScript", "logicComparison");
            IEnumerable<ScriptParameters> metodParameterType = ScriptLibrary.GetMetodParameterType(this.GetScriptName());
            HashSet<string> parameters2 = new HashSet<string>();
            foreach (ScriptParameters item in metodParameterType)
            {
                if (!(item.dropdownType == null) && item.dropdownType.IsSubclassOf(typeof(DBClass)))
                {
                    DataBase.GetType(item.dropdownType).ForEach(delegate(DBClass o)
                    {
                        parameters2.Add(o.dbName);
                    });
                }
            }
            UIComponentFill.LinkDropdown(go, this, "DropdownParameter", "scriptTypeParameter", parameters2.ToList());
            UIComponentFill.LinkInputField<string>(go, this, "InputParameter", "scriptStringParameter");
            break;
        }
        case LRequirement.LERequirementHeroCriteria:
        {
            List<string> avaliableListsOf3 = node.GetAvaliableListsOf(LogicEntry.LEntry.LEEntryHero);
            UIComponentFill.LinkDropdown(go, this, "DropdownListA", "listA", avaliableListsOf3);
            list = ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.EditorHeroResult);
            UIComponentFill.LinkInputField<string>(go, this, "InputParameter", "scriptStringParameter");
            break;
        }
        case LRequirement.LERequirementListSize1:
        {
            List<string> avaliableListsOf2 = node.GetAvaliableListsOf();
            UIComponentFill.LinkDropdown(go, this, "DropdownListA", "listA", avaliableListsOf2);
            break;
        }
        case LRequirement.LERequirementListSize2:
        {
            List<string> avaliableListsOf = node.GetAvaliableListsOf();
            UIComponentFill.LinkDropdown(go, this, "DropdownListA", "listA", avaliableListsOf);
            UIComponentFill.LinkDropdownEnum<LogicUtils.Comparison>(go, this, "DropdownScript", "logicComparison");
            UIComponentFill.LinkInputField<string>(go, this, "InputParameter", "scriptStringParameter");
            break;
        }
        case LRequirement.LERequirementTurn:
            UIComponentFill.LinkToggle(go, this, "ToggleLess", "lessThan");
            UIComponentFill.LinkToggle(go, this, "ToggleEqual", "equal");
            UIComponentFill.LinkToggle(go, this, "ToggleMore", "moreThan");
            UIComponentFill.LinkInputField<string>(go, this, "InputParameter", "scriptStringParameter");
            break;
        case LRequirement.LERequirementWizardCriteria:
            UIComponentFill.LinkDropdownEnum<LogicEntry.PlayerOwner>(go, this, "DropdownListA", "playerOwner");
            list = ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.EditorWizardResult);
            UIComponentFill.LinkInputField<string>(go, this, "InputParameter", "scriptStringParameter");
            break;
        case LRequirement.LERequirementOtherCriteria:
            list = ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.OtherCriteria);
            break;
        default:
            Debug.LogError("unknown case of LRequirement");
            break;
        }
        if (list.Count > 0)
        {
            UIComponentFill.LinkDropdown(go, this, "DropdownScript", "scriptName", list, delegate
            {
                this.PopulateGO(go, node);
            });
            DropDownFilters dropDownFilters = GameObjectUtils.FindByNameGetComponent<DropDownFilters>(go, "DropdownScript");
            if (!dropDownFilters)
            {
                return;
            }
            IEnumerable<ScriptParameters> metodParameterType2 = ScriptLibrary.GetMetodParameterType(dropDownFilters.GetSelection());
            HashSet<string> parameters = new HashSet<string>();
            bool flag = false;
            if (metodParameterType2 != null && metodParameterType2.Count() > 0)
            {
                flag = true;
                foreach (ScriptParameters item2 in metodParameterType2)
                {
                    if (item2.dropdownType == null)
                    {
                        continue;
                    }
                    if (item2.dropdownType.IsSubclassOf(typeof(DBClass)))
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
            }
        }
        UIComponentFill.LinkDropdownEnum<LogicGroupHint>(go, this, "DropdownGroupHint", "group");
    }

    public override string GetScriptName()
    {
        switch (this.logicType)
        {
        case LRequirement.LERequirementChance:
            return "FRA_Chance";
        case LRequirement.LERequirementFame:
            return "FRA_Fame";
        case LRequirement.LERequirementFood:
            return "FRA_Food";
        case LRequirement.LERequirementGold:
            return "FRA_Gold";
        case LRequirement.LERequirementListSize1:
            return "FRA_ListSizeEqual";
        case LRequirement.LERequirementListSize2:
            return "FRA_ListSizeComparison";
        case LRequirement.LERequirementMagic:
            return "FRA_Magic";
        case LRequirement.LERequirementMana:
            return "FRA_Mana";
        case LRequirement.LERequirementTurn:
            return "FRA_Turn";
        default:
            return this.scriptName;
        }
    }
}
