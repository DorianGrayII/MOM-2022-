using System;
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
    public class LogicModifier : AdvLogic
    {
        public enum LModifier
        {
            LEModificationReward = 0,
            LEModificationRewardHero = 1,
            LEModificationRewardItem = 2,
            LEModificationRewardUnits = 3
        }

        [XmlAttribute]
        public LModifier logicType;

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

        [XmlElement]
        [DefaultValue(null)]
        public List<LogicRequirementGroup> requirementGroups;

        [XmlAttribute]
        [DefaultValue(null)]
        public string powerValue;

        [XmlAttribute]
        [DefaultValue(null)]
        public bool powerScalable;

        [XmlAttribute]
        [DefaultValue(null)]
        public string numberOfUnits;

        private static List<string> unitTypes;

        private static List<string> heroesTypes;

        private static List<string> itemTypes;

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
            if (this.logicType == LModifier.LEModificationReward)
            {
                this.PopulateBase(go, node);
            }
            else
            {
                this.PopulateAdds(go, node);
            }
        }

        public void PopulateBase(GameObject go, EditorNode node)
        {
            List<string> metodsNamesOfType = ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.EditorModifier);
            Dictionary<string, LogicEntry.LEntry> avaliableLists = node.GetAvaliableLists();
            UIComponentFill.LinkDropdown(go, this, "DropdownListA", "listA", new List<string>(avaliableLists.Keys));
            UIComponentFill.LinkDropdown(go, this, "DropdownBonusType", "scriptName", metodsNamesOfType, delegate
            {
                this.PopulateGO(go, node);
            });
            UIComponentFill.LinkInputField<string>(go, this, "InputParameter", "scriptStringParameter");
            IEnumerable<ScriptParameters> metodParameterType = ScriptLibrary.GetMetodParameterType(GameObjectUtils.FindByNameGetComponent<DropDownFilters>(go, "DropdownBonusType").GetSelection());
            HashSet<string> parameters = new HashSet<string>();
            bool flag = true;
            if (metodParameterType != null)
            {
                foreach (ScriptParameters item in metodParameterType)
                {
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
        }

        public void PopulateAdds(GameObject go, EditorNode node)
        {
            if (this.logicType != LModifier.LEModificationRewardItem)
            {
                Dictionary<string, LogicEntry.LEntry> avaliableLists = node.GetAvaliableLists();
                UIComponentFill.LinkDropdown(go, this, "DropdownListA", "listA", new List<string>(avaliableLists.Keys));
            }
            GameObjectUtils.FindByNameGetComponentInChildren<Button>(go, "ButtonRequirementGroupAdd").onClick.AddListener(delegate
            {
                if (this.requirementGroups == null)
                {
                    this.requirementGroups = new List<LogicRequirementGroup>();
                }
                this.requirementGroups.Add(new LogicRequirementGroup());
                MHEventSystem.TriggerEvent<AdvLogic>(this, "Update");
            });
            GameObject gameObject = GameObjectUtils.FindByName(go, "ReqGroup", onlyDirectChildren: true);
            RequirementGroupListItem component = gameObject.GetComponent<RequirementGroupListItem>();
            GameObjectUtils.RemoveChildren(gameObject.transform);
            if (this.requirementGroups != null)
            {
                foreach (LogicRequirementGroup v in this.requirementGroups)
                {
                    if (v != null && v.options != null && v.options != null)
                    {
                        for (int i = 0; i < v.options.Count; i++)
                        {
                            int index = i;
                            if (i > 0)
                            {
                                GameObjectUtils.Instantiate(component.labelOr, gameObject.transform);
                            }
                            GameObject gameObject2 = GameObjectUtils.Instantiate(component.option, gameObject.transform);
                            GameObjectUtils.FindByNameGetComponentInChildren<Button>(gameObject2, "ButtonDeleteOption").onClick.AddListener(delegate
                            {
                                v.options.RemoveAt(index);
                                if (v.options.Count == 0)
                                {
                                    this.requirementGroups.Remove(v);
                                }
                                MHEventSystem.TriggerEvent<AdvLogic>(this, "Update");
                            });
                            LogicOptionalGroup o = v.options[index];
                            DropDownFilters dropDownFilters = GameObjectUtils.FindByNameGetComponentInChildren<DropDownFilters>(gameObject2, "DropdownComparison");
                            dropDownFilters.SetOptions(LogicOptionalGroup.signs, doUpdate: false, localize: false);
                            dropDownFilters.SelectOption(o.sign);
                            dropDownFilters.onChange = delegate(object ob)
                            {
                                o.sign = ob.ToString();
                            };
                            UIComponentFill.LinkInputField<int>(gameObject2, o, "InputValue", "value");
                            List<string> collection = null;
                            if (this.logicType == LModifier.LEModificationRewardUnits)
                            {
                                collection = this.UnitTypes();
                            }
                            else if (this.logicType == LModifier.LEModificationRewardHero)
                            {
                                collection = this.HeroesTypes();
                            }
                            else if (this.logicType == LModifier.LEModificationRewardItem)
                            {
                                collection = this.ItemTypes();
                            }
                            UIComponentFill.LinkDropdown(gameObject2, o, "DropdownDatabaseSource", "typeData", new List<string>(collection));
                        }
                    }
                    GameObjectUtils.Instantiate(component.addOptionButton, gameObject.transform).GetComponent<Button>().onClick.AddListener(delegate
                    {
                        if (v.options == null)
                        {
                            v.options = new List<LogicOptionalGroup>();
                        }
                        v.options.Add(new LogicOptionalGroup());
                        MHEventSystem.TriggerEvent<AdvLogic>(this, "Update");
                    });
                    GameObjectUtils.Instantiate(component.divider, gameObject.transform);
                }
            }
            UIComponentFill.LinkInputField<string>(go, this, "InputPowerValue", "powerValue");
            UIComponentFill.LinkToggle(go, this, "ToggleScalable", "powerScalable");
            if (this.logicType == LModifier.LEModificationRewardUnits || this.logicType == LModifier.LEModificationRewardItem)
            {
                UIComponentFill.LinkInputField<string>(go, this, "InputNumberOfUnits", "numberOfUnits");
            }
        }

        public List<string> UnitTypes()
        {
            if (LogicModifier.unitTypes == null)
            {
                LogicModifier.unitTypes = new List<string>();
                foreach (global::DBDef.Unit item in DataBase.GetType<global::DBDef.Unit>())
                {
                    LogicModifier.unitTypes.Add(item.dbName);
                }
                foreach (Race item2 in DataBase.GetType<Race>())
                {
                    LogicModifier.unitTypes.Add(item2.dbName);
                }
                foreach (Tag item3 in DataBase.GetType<Tag>())
                {
                    LogicModifier.unitTypes.Add(item3.dbName);
                }
            }
            return LogicModifier.unitTypes;
        }

        public List<string> HeroesTypes()
        {
            if (LogicModifier.heroesTypes == null)
            {
                LogicModifier.heroesTypes = new List<string>();
                foreach (Hero item in DataBase.GetType<Hero>())
                {
                    LogicModifier.heroesTypes.Add(item.dbName);
                }
                foreach (Tag item2 in DataBase.GetType<Tag>())
                {
                    LogicModifier.heroesTypes.Add(item2.dbName);
                }
            }
            return LogicModifier.heroesTypes;
        }

        public List<string> ItemTypes()
        {
            if (LogicModifier.itemTypes == null)
            {
                LogicModifier.itemTypes = new List<string>();
                string[] names = Enum.GetNames(typeof(EEquipmentType));
                foreach (string text in names)
                {
                    LogicModifier.itemTypes.Add(text.ToString());
                }
                foreach (ArtefactPower item in DataBase.GetType<ArtefactPower>())
                {
                    LogicModifier.itemTypes.Add(item.dbName);
                }
                foreach (global::DBDef.Artefact item2 in DataBase.GetType<global::DBDef.Artefact>())
                {
                    LogicModifier.itemTypes.Add(item2.dbName);
                }
            }
            return LogicModifier.itemTypes;
        }

        public override string GetScriptName()
        {
            switch (this.logicType)
            {
            case LModifier.LEModificationRewardHero:
                return "FMO_AddHero";
            case LModifier.LEModificationRewardUnits:
                return "FMO_AddUnit";
            case LModifier.LEModificationRewardItem:
                return "FMO_AddEquipment";
            default:
                return this.scriptName;
            }
        }
    }
}
