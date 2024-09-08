// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.Adventures.BaseNode
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using DBDef;
using MHUtils;
using MOM;
using MOM.Adventures;
using UnityEngine;

[XmlInclude(typeof(NodeStart))]
[XmlInclude(typeof(NodeStory))]
[XmlInclude(typeof(NodeEnd))]
[XmlInclude(typeof(NodeBattle))]
[XmlInclude(typeof(NodeTrade))]
[XmlInclude(typeof(NodeRelay))]
[XmlInclude(typeof(NodeSpawnLocation))]
[XmlInclude(typeof(NodeWorldEnchantment))]
[XmlInclude(typeof(NodeLocationAward))]
public abstract class BaseNode
{
    [XmlAttribute]
    [DefaultValue(false)]
    public bool allowOnce;

    [XmlAttribute]
    public int ID;

    [XmlAttribute]
    public float positionX;

    [XmlAttribute]
    public float positionY;

    [XmlAttribute]
    [DefaultValue(null)]
    public string image;

    [XmlElement]
    [DefaultValue(null)]
    public List<AdvLogic> logic;

    [XmlElement]
    [DefaultValue(null)]
    public List<AdvOutput> outputs;

    [XmlIgnore]
    protected HashSet<BaseNode> inputs;

    [XmlIgnore]
    public Adventure parentEvent;

    [XmlIgnore]
    public bool changed;

    [XmlIgnore]
    public float PositionX
    {
        get
        {
            return this.positionX;
        }
        set
        {
            this.positionX = (int)value;
        }
    }

    [XmlIgnore]
    public float PositionY
    {
        get
        {
            return this.positionY;
        }
        set
        {
            this.positionY = (int)value;
        }
    }

    public BaseNode()
    {
    }

    public AdvOutput AddOutput(string text = null)
    {
        if (this.outputs == null)
        {
            this.outputs = new List<AdvOutput>();
        }
        AdvOutput advOutput = new AdvOutput();
        advOutput.ownerID = this.ID;
        this.outputs.Add(advOutput);
        if (text != null)
        {
            advOutput.name = text;
        }
        this.changed = true;
        return advOutput;
    }

    internal void AddLogic(string v)
    {
        if (v != null)
        {
            if (this.logic == null)
            {
                this.logic = new List<AdvLogic>();
            }
            if (v.StartsWith("LEE"))
            {
                LogicEntry logicEntry = new LogicEntry();
                logicEntry.logicType = (LogicEntry.LEntry)Enum.Parse(typeof(LogicEntry.LEntry), v, ignoreCase: true);
                this.logic.Add(logicEntry);
            }
            else if (v.StartsWith("LEP"))
            {
                LogicProcessing logicProcessing = new LogicProcessing();
                logicProcessing.logicType = (LogicProcessing.LProcessing)Enum.Parse(typeof(LogicProcessing.LProcessing), v, ignoreCase: true);
                this.logic.Add(logicProcessing);
            }
            else if (v.StartsWith("LER"))
            {
                LogicRequirement logicRequirement = new LogicRequirement();
                logicRequirement.logicType = (LogicRequirement.LRequirement)Enum.Parse(typeof(LogicRequirement.LRequirement), v, ignoreCase: true);
                this.logic.Add(logicRequirement);
            }
            else if (v.StartsWith("LEM"))
            {
                LogicModifier logicModifier = new LogicModifier();
                logicModifier.logicType = (LogicModifier.LModifier)Enum.Parse(typeof(LogicModifier.LModifier), v, ignoreCase: true);
                this.logic.Add(logicModifier);
            }
        }
    }

    public void RemoveOutput(int index)
    {
        if (this.outputs != null)
        {
            this.changed = true;
            this.outputs.RemoveAt(index);
        }
    }

    public void RemoveOutput(AdvOutput output)
    {
        if (this.outputs != null)
        {
            this.changed = true;
            this.outputs.Remove(output);
        }
    }

    public virtual HashSet<BaseNode> GetInputs()
    {
        if (this.inputs == null)
        {
            this.inputs = new HashSet<BaseNode>();
        }
        return this.inputs;
    }

    public Vector2 GetSize(Dictionary<Type, Vector2> sizeTable)
    {
        Type type = base.GetType();
        foreach (KeyValuePair<Type, Vector2> item in sizeTable)
        {
            if (item.Key == type)
            {
                return item.Value;
            }
        }
        return Vector2.zero;
    }

    internal HashSet<string> GetLocalAvaliableListNames(bool onlyPublic, bool onlyPrivate)
    {
        HashSet<string> hashSet = new HashSet<string>();
        if (this.logic != null && this.logic.Count > 0)
        {
            foreach (AdvLogic item in this.logic)
            {
                if (item is LogicEntry)
                {
                    LogicEntry logicEntry = item as LogicEntry;
                    if (!string.IsNullOrEmpty(logicEntry.listName) && !hashSet.Contains(logicEntry.listName) && ((!onlyPublic && !onlyPrivate) || (onlyPublic && logicEntry.makeListPublic) || (onlyPrivate && !logicEntry.makeListPublic)))
                    {
                        hashSet.Add(logicEntry.listName);
                    }
                }
                if (item is LogicProcessing)
                {
                    LogicProcessing logicProcessing = item as LogicProcessing;
                    if (!string.IsNullOrEmpty(logicProcessing.listName) && !hashSet.Contains(logicProcessing.listName) && ((!onlyPublic && !onlyPrivate) || (onlyPublic && logicProcessing.makeListPublic) || (onlyPrivate && !logicProcessing.makeListPublic)))
                    {
                        hashSet.Add(logicProcessing.listName);
                    }
                }
            }
        }
        return hashSet;
    }

    public virtual bool RequiresOutputText()
    {
        return false;
    }

    public void PrepareForEdit(Adventure parent)
    {
        this.parentEvent = parent;
        if (this.outputs == null)
        {
            return;
        }
        foreach (AdvOutput output in this.outputs)
        {
            BaseNode target = output.GetTarget(this.parentEvent);
            if (target != null)
            {
                if (target.inputs == null)
                {
                    target.inputs = new HashSet<BaseNode>();
                }
                target.inputs.Add(this);
            }
        }
    }

    public abstract void InitializeOutputs();

    public abstract void UpdateVisuals(EditorNode editorNode);

    public int GetOutputCount()
    {
        if (this.outputs == null)
        {
            return 0;
        }
        return this.outputs.Count;
    }

    public string GetGraphic()
    {
        string text = this.image;
        if (string.IsNullOrEmpty(text))
        {
            HashSet<BaseNode> visitedNodes = new HashSet<BaseNode>();
            text = this.SeekImage(visitedNodes);
        }
        return text;
    }

    protected string SeekImage(HashSet<BaseNode> visitedNodes)
    {
        visitedNodes.Add(this);
        if (string.IsNullOrEmpty(this.image) && this.inputs != null)
        {
            foreach (BaseNode input in this.inputs)
            {
                string text = input.SeekImage(visitedNodes);
                if (!string.IsNullOrEmpty(text))
                {
                    return text;
                }
            }
        }
        return this.image;
    }

    public void DisconnectAll()
    {
        if (this.outputs != null)
        {
            foreach (AdvOutput output in this.outputs)
            {
                output.Disconnect();
            }
        }
        if (this.inputs == null)
        {
            return;
        }
        foreach (BaseNode item in new List<BaseNode>(this.inputs))
        {
            item.DisconnectOutputTo(this);
        }
    }

    public void DisconnectOutputTo(BaseNode n)
    {
        if (this.outputs == null)
        {
            return;
        }
        foreach (AdvOutput output in this.outputs)
        {
            if (output.GetTarget(EventEditorAdventures.selectedAdventure) == n)
            {
                output.Disconnect();
            }
        }
    }

    public bool HaveModifiers()
    {
        if (this.logic != null)
        {
            foreach (AdvLogic item in this.logic)
            {
                if (item is LogicModifier)
                {
                    return true;
                }
            }
        }
        return false;
    }

    internal bool HaveFilter()
    {
        if (this.logic != null)
        {
            foreach (AdvLogic item in this.logic)
            {
                if (item is LogicRequirement)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public virtual int Test(Adventure eventOwner)
    {
        int num = 0;
        if (this.logic != null)
        {
            foreach (AdvLogic item in this.logic)
            {
                if (item == null)
                {
                    Debug.LogError("[ERROR][EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " have ERRERNEUS logic element, report to Khash: bugs@muhagames.com ");
                    continue;
                }
                string text = null;
                string text2 = null;
                if (item is LogicProcessing)
                {
                    LogicProcessing obj = item as LogicProcessing;
                    text = obj.listA;
                    text2 = obj.listB;
                }
                if (item is LogicModifier)
                {
                    text = (item as LogicModifier).listA;
                }
                if (item is LogicRequirement)
                {
                    text = (item as LogicRequirement).listA;
                }
                if (!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(text2))
                {
                    HashSet<string> avaliableListNames = eventOwner.GetAvaliableListNames(this);
                    if (!string.IsNullOrEmpty(text))
                    {
                        if (!avaliableListNames.Contains(text) && text != LogicEntry.LEntry.NONE.ToString())
                        {
                            Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " have logic using list: " + text + " which cannot be found ");
                        }
                        for (int i = this.logic.IndexOf(item); i < this.logic.Count; i++)
                        {
                            if (this.logic[i] is LogicProcessing && !(item is LogicRequirement) && !(item is LogicModifier) && (this.logic[i] as LogicProcessing).listName == text)
                            {
                                Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " have logic using list: " + text + " which seems to be created later in this node! ");
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(text2))
                    {
                        if (!avaliableListNames.Contains(text2) && text2 != LogicEntry.LEntry.NONE.ToString())
                        {
                            Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " have logic using list: " + text2 + " which cannot be found ");
                        }
                        for (int j = this.logic.IndexOf(item); j < this.logic.Count; j++)
                        {
                            if (this.logic[j] is LogicProcessing && !(item is LogicRequirement) && !(item is LogicModifier) && (this.logic[j] as LogicProcessing).listName == text2)
                            {
                                Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " have logic using list: " + text2 + " which seems to be created later in this node! ");
                            }
                        }
                    }
                }
                string value = null;
                string text3 = null;
                string text4 = null;
                string typeParameter = null;
                List<LogicRequirementGroup> list = null;
                bool flag = false;
                bool flag2 = false;
                bool flag3 = false;
                bool flag4 = false;
                bool flag5 = false;
                bool flag6 = false;
                bool flag7 = false;
                if (item is LogicEntry)
                {
                    LogicEntry logicEntry = item as LogicEntry;
                    text3 = logicEntry.scriptName;
                    text4 = logicEntry.scriptStringParameter;
                    typeParameter = logicEntry.scriptTypeParameter;
                    value = logicEntry.listName;
                    flag2 = true;
                    flag = true;
                }
                else if (item is LogicProcessing)
                {
                    LogicProcessing logicProcessing = item as LogicProcessing;
                    text3 = logicProcessing.scriptName;
                    text4 = logicProcessing.scriptStringParameter;
                    typeParameter = logicProcessing.scriptTypeParameter;
                    value = logicProcessing.listName;
                    flag2 = true;
                    if (logicProcessing.logicType == LogicProcessing.LProcessing.LEProcessingAdd || logicProcessing.logicType == LogicProcessing.LProcessing.LEProcessingMultiply || logicProcessing.logicType == LogicProcessing.LProcessing.LEProcessingSubtract)
                    {
                        flag3 = true;
                        flag4 = true;
                    }
                    else if (logicProcessing.logicType == LogicProcessing.LProcessing.LEProcessingFilter)
                    {
                        flag3 = true;
                        flag = true;
                    }
                }
                else if (item is LogicRequirement)
                {
                    LogicRequirement logicRequirement = item as LogicRequirement;
                    text3 = logicRequirement.scriptName;
                    text4 = logicRequirement.scriptStringParameter;
                    typeParameter = logicRequirement.scriptTypeParameter;
                    if (logicRequirement.logicType == LogicRequirement.LRequirement.LERequirementChance)
                    {
                        if (string.IsNullOrEmpty(logicRequirement.scriptStringValue))
                        {
                            Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " is missing obligatory parameter value");
                        }
                    }
                    else if (logicRequirement.logicType == LogicRequirement.LRequirement.LERequirementCheckSharedTag || logicRequirement.logicType == LogicRequirement.LRequirement.LERequirementFamiliar)
                    {
                        flag = true;
                        flag6 = true;
                    }
                    else if (logicRequirement.logicType == LogicRequirement.LRequirement.LERequirementCityCriteria || logicRequirement.logicType == LogicRequirement.LRequirement.LERequirementHeroCriteria)
                    {
                        flag3 = true;
                        flag = true;
                    }
                    else if (logicRequirement.logicType == LogicRequirement.LRequirement.LERequirementFood || logicRequirement.logicType == LogicRequirement.LRequirement.LERequirementGold || logicRequirement.logicType == LogicRequirement.LRequirement.LERequirementMana || logicRequirement.logicType == LogicRequirement.LRequirement.LERequirementTurn)
                    {
                        flag5 = true;
                    }
                    else if (logicRequirement.logicType == LogicRequirement.LRequirement.LERequirementListSize1)
                    {
                        flag3 = true;
                    }
                    else if (logicRequirement.logicType == LogicRequirement.LRequirement.LERequirementListSize2)
                    {
                        flag3 = true;
                        flag5 = true;
                    }
                    else if (logicRequirement.logicType == LogicRequirement.LRequirement.LERequirementWizardCriteria || logicRequirement.logicType == LogicRequirement.LRequirement.LERequirementOtherCriteria)
                    {
                        flag = true;
                    }
                }
                else if (item is LogicModifier)
                {
                    LogicModifier logicModifier = item as LogicModifier;
                    text3 = logicModifier.scriptName;
                    text4 = logicModifier.scriptStringParameter;
                    typeParameter = logicModifier.scriptTypeParameter;
                    if (logicModifier.logicType == LogicModifier.LModifier.LEModificationReward)
                    {
                        flag3 = true;
                        flag = true;
                    }
                    else if (logicModifier.logicType == LogicModifier.LModifier.LEModificationRewardHero)
                    {
                        flag3 = true;
                        flag7 = true;
                        list = logicModifier.requirementGroups;
                    }
                    else if (logicModifier.logicType == LogicModifier.LModifier.LEModificationRewardUnits)
                    {
                        flag3 = true;
                    }
                }
                if (flag && string.IsNullOrEmpty(text3))
                {
                    Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " is missing obligatory script");
                }
                if (flag2 && string.IsNullOrEmpty(value))
                {
                    Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " is missing obligatory list name");
                }
                if (flag3 && string.IsNullOrEmpty(text))
                {
                    Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " is missing obligatory ListA");
                }
                if (flag4 && string.IsNullOrEmpty(text2))
                {
                    Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " is missing obligatory ListB");
                }
                if (flag6 && string.IsNullOrEmpty(typeParameter))
                {
                    Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " is missing obligatory type parameter");
                }
                if (flag5 && string.IsNullOrEmpty(text4))
                {
                    Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " is missing obligatory string parameter");
                }
                if (flag7)
                {
                    if (list == null || list.Count == 0)
                    {
                        Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " is missing obligatory reward requirement");
                    }
                    else
                    {
                        bool flag8 = true;
                        foreach (LogicRequirementGroup item2 in list)
                        {
                            if (!flag8)
                            {
                                break;
                            }
                            foreach (LogicOptionalGroup option in item2.options)
                            {
                                if (option.typeData != null)
                                {
                                    flag8 = false;
                                    break;
                                }
                            }
                        }
                        if (flag8)
                        {
                            Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " obligatory reward requirement is empty");
                        }
                    }
                }
                if (string.IsNullOrEmpty(text3))
                {
                    continue;
                }
                MethodInfo methodInfo = ScriptLibrary.Get(text3);
                if (methodInfo == null)
                {
                    Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " contains Logic node which uses unknown script: " + text3);
                    continue;
                }
                DBClass dBClass = null;
                if (!string.IsNullOrEmpty(typeParameter))
                {
                    dBClass = DataBase.Get(typeParameter, reportMissing: false);
                }
                object[] customAttributes = methodInfo.GetCustomAttributes(inherit: false);
                bool flag9 = customAttributes.Length == 0;
                Type type = null;
                object[] array = customAttributes;
                foreach (object obj2 in array)
                {
                    if (!(obj2 is ScriptParameters))
                    {
                        continue;
                    }
                    ScriptParameters scriptParameters = obj2 as ScriptParameters;
                    if (dBClass == null)
                    {
                        if (scriptParameters.dropdownType == null)
                        {
                            flag9 = true;
                        }
                        else if (scriptParameters.dropdownType.IsEnum && Array.Find(Enum.GetNames(scriptParameters.dropdownType), (string o) => o == typeParameter) != null)
                        {
                            flag9 = true;
                        }
                    }
                    else if (dBClass.GetType() == scriptParameters.dropdownType)
                    {
                        flag9 = true;
                    }
                    type = scriptParameters.variableType;
                    if (type != null && !string.IsNullOrEmpty(text4))
                    {
                        if (type == typeof(int))
                        {
                            bool flag10 = false;
                            try
                            {
                                Convert.ToInt32(text4);
                                flag10 = true;
                            }
                            catch
                            {
                            }
                            if (!flag10)
                            {
                                Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " contains Logic node witch script: " + text3 + " which have invalid text parameter " + text4 + ", expected: INTEGER, eg: 5");
                            }
                        }
                        else if (type == typeof(FInt))
                        {
                            bool flag11 = false;
                            try
                            {
                                Convert.ToSingle(text4, Globals.floatingPointCultureInfo);
                                flag11 = true;
                            }
                            catch
                            {
                            }
                            if (!flag11)
                            {
                                Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " contains Logic node witch script: " + text3 + " which have invalid text parameter " + text4 + ", expected: FLOAT, eg: 12 or 1.6 or .7");
                            }
                        }
                        else if (type == typeof(Multitype<float, float, float>) && (Multitype<float, float, float>)ScriptLibrary.Call("UTIL_StringParameterProcessor", text4) == null)
                        {
                            Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " contains Logic node witch script: " + text3 + " which have invalid text parameter " + text4 + ", expected special type which should contain one or more parts like: 15% or 50%;3.4 or 10%;12;40 There is one part which provides percentage (which is used by some scripts only) and two numbers of which earlier number is seen as minimum and another as maximum.");
                        }
                    }
                    if (!flag9)
                    {
                        Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " contains Logic node witch script: " + text3 + " which have invalid parameter " + typeParameter);
                    }
                }
            }
        }
        if (this is NodeBattle)
        {
            NodeBattle nodeBattle = this as NodeBattle;
            if (string.IsNullOrEmpty(nodeBattle.opponentGroupName) && string.IsNullOrEmpty(nodeBattle.scriptName))
            {
                Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " is lacking of opponent group and opponent group creation script. Node have to contain one of them");
            }
            if (!string.IsNullOrEmpty(nodeBattle.scriptName) && ScriptLibrary.Get(nodeBattle.scriptName) == null)
            {
                Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " contains Logic node which uses unknown script: " + nodeBattle.scriptName);
            }
            if (!string.IsNullOrEmpty(nodeBattle.listA) && nodeBattle.listA != LogicEntry.LEntry.NONE.ToString() && !eventOwner.GetAvaliableListNames(this).Contains(nodeBattle.listA))
            {
                Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " is using list: " + nodeBattle.listA + " which cannot be found ");
            }
        }
        if (this is NodeWorldEnchantment)
        {
            NodeWorldEnchantment nodeWorldEnchantment = this as NodeWorldEnchantment;
            if (string.IsNullOrEmpty(nodeWorldEnchantment.targetName))
            {
                Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " is lacking of Enchantment target");
            }
            if (string.IsNullOrEmpty(nodeWorldEnchantment.enchantmentName))
            {
                Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " is lacking of Enchantment name");
            }
            else if (DataBase.Get(nodeWorldEnchantment.enchantmentName, reportMissing: false) == null)
            {
                Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " contains unknown enchantment: " + nodeWorldEnchantment.enchantmentName);
            }
        }
        if (this is NodeSpawnLocation)
        {
            NodeSpawnLocation ns2 = this as NodeSpawnLocation;
            if (!ns2.destroyOwner)
            {
                if (string.IsNullOrEmpty(ns2.spawnName))
                {
                    Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " Is not a destruction node, but does not specify any settlement.");
                }
                if (string.IsNullOrEmpty(ns2.anchorName))
                {
                    Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " Is not a destruction node, but is lacking of spawn anchor.");
                }
                else if (!eventOwner.GetAvaliableListNames(this).Contains(ns2.anchorName))
                {
                    Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " is using list: " + ns2.anchorName + " which cannot be found ");
                }
                if (string.IsNullOrEmpty(ns2.scriptName))
                {
                    Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " Is not a destruction node, but is lacking script name.");
                }
                else if (ScriptLibrary.Get(ns2.scriptName) == null)
                {
                    Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " contains Logic node which uses unknown script: " + ns2.scriptName);
                }
                if (!string.IsNullOrEmpty(ns2.distance) && (Multitype<float, float, float>)ScriptLibrary.Call("UTIL_StringParameterProcessor", ns2.distance) == null)
                {
                    Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " which have invalid Distance parameter: " + ns2.distance + ", expected special type which should contain one or more parts like: 15% or 50%;3.4 or 10%;12;40 There is one part which provides percentage (which is used by some scripts only) and two numbers of which earlier number is seen as minimum and another as maximum.");
                }
                if (ns2.navigateToEvent > 0 && eventOwner.module.adventures.Find((Adventure o) => o.uniqueID == ns2.navigateToEvent) == null)
                {
                    Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " refers to the event ID: " + ns2.navigateToEvent + " which is missing in this module! ");
                }
            }
        }
        _ = this is NodeTrade;
        if (!(this is NodeEnd))
        {
            if (this.outputs == null || this.outputs.Count < 1)
            {
                Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " lacks Outputs!");
                num++;
            }
            else
            {
                for (int l = 0; l < this.outputs.Count; l++)
                {
                    num += this.outputs[l].Test(this);
                    for (int m = l + 1; m < this.outputs.Count; m++)
                    {
                        if (this.outputs[l].ownerID == this.outputs[m].ownerID && this.outputs[l].targetID == this.outputs[m].targetID && this.outputs[l].group == this.outputs[m].group && this.outputs[l].name == this.outputs[m].name)
                        {
                            Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " have duplicate erroneous outputs! (name, from and to is identical)");
                        }
                    }
                }
                bool flag12 = false;
                for (int n = 0; n < this.outputs.Count; n++)
                {
                    BaseNode target = this.outputs[n].GetTarget(eventOwner);
                    if ((!(target is NodeStory) || !(target as NodeStory).allowOnce) && target != null && !target.HaveFilter())
                    {
                        flag12 = true;
                    }
                }
                if (!flag12)
                {
                    Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " does not offer any path without filters and it is possible that no path would be available!");
                }
            }
        }
        else
        {
            NodeEnd ns = this as NodeEnd;
            if (ns.navigateToEvent > 0 && eventOwner.module.adventures.Find((Adventure o) => o.uniqueID == ns.navigateToEvent) == null)
            {
                Debug.LogWarning("[EDITOR] Node " + this.ID + " in event (" + this.parentEvent.uniqueID + ")" + this.parentEvent.name + " in module " + this.parentEvent.module.name + " refers to the event ID: " + ns.navigateToEvent + " which is missing in this module! ");
            }
        }
        return num;
    }
}
