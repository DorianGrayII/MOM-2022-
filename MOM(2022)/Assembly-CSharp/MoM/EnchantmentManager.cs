// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.EnchantmentManager
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DBDef;
using MHUtils;
using MHUtils.UI;
using MOM;
using ProtoBuf;
using UnityEngine;

[ProtoContract]
public class EnchantmentManager
{
    private class ScriptIterator
    {
        private static List<List<ScriptIterator>> currentIterators;

        private EEnchantmentType eType;

        public int next;

        public int current;

        public List<EnchantmentScript> list;

        static ScriptIterator()
        {
            ScriptIterator.currentIterators = new List<List<ScriptIterator>>();
            int num = Enum.GetValues(typeof(EEnchantmentType)).Cast<int>().Max() + 1;
            ScriptIterator.currentIterators = new List<List<ScriptIterator>>(num);
            for (int i = 0; i < num; i++)
            {
                ScriptIterator.currentIterators.Add(new List<ScriptIterator>(1));
            }
        }

        public static bool ItemRemoved(EEnchantmentType type, int index, List<EnchantmentScript> list)
        {
            List<ScriptIterator> obj = ScriptIterator.currentIterators[(int)type];
            bool result = false;
            foreach (ScriptIterator item in obj)
            {
                if (item.list == list)
                {
                    item.InternalItemRemoved(index);
                    result = true;
                }
            }
            return result;
        }

        public ScriptIterator(EEnchantmentType eType, List<EnchantmentScript> list)
        {
            this.eType = eType;
            this.list = list;
            this.current = ((list != null) ? (list.Count - 1) : (-1));
            this.next = this.current - 1;
            if (this.current >= 0)
            {
                ScriptIterator.currentIterators[(int)eType].Add(this);
            }
        }

        public EnchantmentScript Current()
        {
            if (this.current >= 0)
            {
                return this.list[this.current];
            }
            return null;
        }

        public void Next()
        {
            this.current = this.next;
            this.next--;
            if (this.current < 0)
            {
                ScriptIterator.currentIterators[(int)this.eType].Remove(this);
                this.list = null;
            }
        }

        private void InternalItemRemoved(int index)
        {
            if (this.current == index)
            {
                this.current = -1;
            }
            else if (this.current > index)
            {
                this.current--;
            }
            if (this.next >= index)
            {
                this.next--;
            }
            if (this.next < 0 && this.current < 0)
            {
                ScriptIterator.currentIterators[(int)this.eType].Remove(this);
                this.list = null;
            }
        }

        public void End()
        {
            ScriptIterator.currentIterators[(int)this.eType].Remove(this);
            this.next = -1;
            this.current = -1;
            this.list = null;
        }
    }

    [ProtoIgnore]
    public IEnchantable owner;

    [ProtoIgnore]
    public Dictionary<EEnchantmentType, List<EnchantmentScript>> scripts = new Dictionary<EEnchantmentType, List<EnchantmentScript>>();

    [ProtoIgnore]
    public Dictionary<EnchantmentScript, EnchantmentInstance> scriptToinstance = new Dictionary<EnchantmentScript, EnchantmentInstance>();

    [ProtoIgnore]
    public List<EnchantmentInstance> onJoinTriggers;

    [ProtoIgnore]
    public List<EnchantmentInstance> onLeaveTriggers;

    [ProtoIgnore]
    public int iteration;

    [ProtoIgnore]
    public int localActiveIterators;

    [ProtoMember(1)]
    public List<EnchantmentInstance> enchantments = new List<EnchantmentInstance>();

    public EnchantmentManager()
    {
    }

    public EnchantmentManager(IEnchantable owner)
    {
        this.owner = owner;
    }

    ~EnchantmentManager()
    {
        if (!EnchantmentRegister.IsInstantiated() || (this.owner is BattleUnit || (this.owner is BattlePlayer || (this.owner is Battle || this.enchantments == null))))
        {
            return;
        }
        foreach (EnchantmentInstance enchantment in this.enchantments)
        {
            EnchantmentRegister.EnchantmentRemoved(enchantment);
        }
    }

    [ProtoAfterDeserialization]
    public void Reinitialization()
    {
        this.EnsureDictionaries();
        this.EnsureJoinLeaveLists();
        this.SetEnchantmentManagers();
    }

    public EnchantmentInstance Add(Enchantment effect, Entity owner = null, int countDown = -1, string parameters = null, bool inBattle = false, int dispelcost = 0)
    {
        this.iteration++;
        EnchantmentInstance enchantmentInstance = new EnchantmentInstance();
        enchantmentInstance.nameID = effect.dbName;
        enchantmentInstance.countDown = countDown;
        enchantmentInstance.manager = this;
        enchantmentInstance.owner = ((owner != null) ? new Reference(owner) : null);
        enchantmentInstance.source = effect;
        enchantmentInstance.parameters = parameters;
        enchantmentInstance.battleEnchantment = inBattle;
        enchantmentInstance.dispelCost = dispelcost;
        enchantmentInstance.upkeepMana = effect.upkeepCost;
        this.enchantments.Add(enchantmentInstance);
        if (effect.scripts != null)
        {
            EnchantmentScript[] array = effect.scripts;
            foreach (EnchantmentScript enchantmentScript in array)
            {
                if (!this.scripts.ContainsKey(enchantmentScript.triggerType))
                {
                    this.scripts[enchantmentScript.triggerType] = new List<EnchantmentScript>();
                }
                this.scripts[enchantmentScript.triggerType].Add(enchantmentScript);
                enchantmentInstance.enchantmentHolder = ((this.owner.GetWizardOwner() != null) ? this.owner.GetWizardOwner().ID : 0);
                this.scriptToinstance[enchantmentScript] = enchantmentInstance;
                if (enchantmentScript.triggerType == EEnchantmentType.VisibilityRangeModifier && this.owner is IPlanePosition planePosition)
                {
                    FOW.Get().UpdateFogForPlane(planePosition.GetPlane());
                }
            }
        }
        if (!string.IsNullOrEmpty(effect.onJoinWithUnit))
        {
            if (this.onJoinTriggers == null)
            {
                this.onJoinTriggers = new List<EnchantmentInstance>();
            }
            this.onJoinTriggers.Add(enchantmentInstance);
        }
        if (!string.IsNullOrEmpty(effect.onLeaveFromUnit))
        {
            if (this.onLeaveTriggers == null)
            {
                this.onLeaveTriggers = new List<EnchantmentInstance>();
            }
            this.onLeaveTriggers.Add(enchantmentInstance);
        }
        if (this.owner is TownLocation)
        {
            VerticalMarkerManager.Get().UpdateInfoOnMarker(this.owner);
            (this.owner as TownLocation).UpdateOwnerModels();
        }
        if (this.owner is Battle || this.owner is BattlePlayer || this.owner is BattleUnit)
        {
            BattleHUD.Get()?.Dirty();
        }
        else if (!inBattle)
        {
            EnchantmentRegister.EnchantmentAdded(enchantmentInstance);
        }
        return enchantmentInstance;
    }

    public EnchantmentInstance Add2(EnchantmentInstance ei, bool inBattle = false)
    {
        this.iteration++;
        ei.battleEnchantment = inBattle;
        ei.manager = this;
        this.enchantments.Add(ei);
        if (ei.source.Get().scripts != null)
        {
            EnchantmentScript[] array = ei.source.Get().scripts;
            foreach (EnchantmentScript enchantmentScript in array)
            {
                if (!this.scripts.ContainsKey(enchantmentScript.triggerType))
                {
                    this.scripts[enchantmentScript.triggerType] = new List<EnchantmentScript>();
                }
                this.scripts[enchantmentScript.triggerType].Add(enchantmentScript);
                this.scriptToinstance[enchantmentScript] = ei;
            }
        }
        if (!string.IsNullOrEmpty(ei.source.Get().onJoinWithUnit))
        {
            if (this.onJoinTriggers == null)
            {
                this.onJoinTriggers = new List<EnchantmentInstance>();
            }
            this.onJoinTriggers.Add(ei);
        }
        if (!string.IsNullOrEmpty(ei.source.Get().onLeaveFromUnit))
        {
            if (this.onLeaveTriggers == null)
            {
                this.onLeaveTriggers = new List<EnchantmentInstance>();
            }
            this.onLeaveTriggers.Add(ei);
        }
        if (this.owner is TownLocation townLocation)
        {
            VerticalMarkerManager.Get().UpdateInfoOnMarker(this.owner);
            townLocation.EnchantmentsChanged();
        }
        if (this.owner is Battle || this.owner is BattlePlayer || this.owner is BattleUnit)
        {
            BattleHUD.Get()?.Dirty();
        }
        else if (!inBattle)
        {
            EnchantmentRegister.EnchantmentAdded(ei);
        }
        return ei;
    }

    public EnchantmentInstance Remove(Enchantment effect)
    {
        this.iteration++;
        for (int i = 0; i < this.enchantments.Count; i++)
        {
            if (this.enchantments[i].source.Get() == effect)
            {
                MHEventSystem.TriggerEvent("RemoveEnchantment", this, this.enchantments[i]);
                return this.Remove(this.enchantments[i]);
            }
        }
        if (this.owner is TownLocation townLocation)
        {
            townLocation.EnchantmentsChanged();
        }
        if (this.owner is Battle || this.owner is BattlePlayer || this.owner is BattleUnit)
        {
            BattleHUD.Get()?.Dirty();
        }
        return null;
    }

    public EnchantmentInstance Remove(EnchantmentInstance ei)
    {
        this.iteration++;
        while (true)
        {
            EnchantmentScript enchantmentScript = null;
            foreach (KeyValuePair<EnchantmentScript, EnchantmentInstance> item in this.scriptToinstance)
            {
                if (item.Value == ei)
                {
                    enchantmentScript = item.Key;
                    break;
                }
            }
            if (enchantmentScript == null)
            {
                break;
            }
            this.scriptToinstance.Remove(enchantmentScript);
            EEnchantmentType triggerType = enchantmentScript.triggerType;
            if (!this.scripts.TryGetValue(triggerType, out var value) || value == null)
            {
                continue;
            }
            int num = value.IndexOf(enchantmentScript);
            if (num >= 0)
            {
                value.RemoveAt(num);
                if (ScriptIterator.ItemRemoved(triggerType, num, value))
                {
                    Debug.LogError("Removal of enchantment " + ei.source.dbName + " during iteration");
                }
            }
        }
        this.enchantments.Remove(ei);
        if (!string.IsNullOrEmpty(ei.source.Get().onJoinWithUnit) && this.onJoinTriggers != null)
        {
            this.onJoinTriggers.Remove(ei);
            if (this.onJoinTriggers.Count == 0)
            {
                this.onJoinTriggers = null;
            }
        }
        if (!string.IsNullOrEmpty(ei.source.Get().onLeaveFromUnit) && this.onLeaveTriggers != null)
        {
            this.onLeaveTriggers.Remove(ei);
            if (this.onLeaveTriggers.Count == 0)
            {
                this.onLeaveTriggers = null;
            }
        }
        if (this.owner is TownLocation townLocation)
        {
            VerticalMarkerManager.Get().UpdateInfoOnMarker(this.owner);
            townLocation.EnchantmentsChanged();
        }
        EnchantmentRegister.EnchantmentRemoved(ei);
        if (this.owner is Battle || this.owner is BattlePlayer || this.owner is BattleUnit)
        {
            BattleHUD.Get()?.Dirty();
        }
        return ei;
    }

    public void TriggerScripts(EEnchantmentType eType, object data, IEnchantable customTarget = null)
    {
        if (this.scripts == null || !this.scripts.ContainsKey(eType))
        {
            return;
        }
        IEnchantable enchantable = customTarget;
        if (enchantable == null)
        {
            enchantable = this.owner;
        }
        this.localActiveIterators++;
        for (ScriptIterator scriptIterator = this.StartIteration(eType); scriptIterator.Current() != null; scriptIterator.Next())
        {
            EnchantmentScript enchantmentScript = scriptIterator.Current();
            if (enchantmentScript == null)
            {
                Debug.LogError("null script in the dictionary of type : " + eType);
            }
            if (!this.scriptToinstance.ContainsKey(enchantmentScript))
            {
                continue;
            }
            EnchantmentInstance enchantmentInstance = this.scriptToinstance[enchantmentScript];
            if (eType == EEnchantmentType.RemoteUnitAttributeChange || eType == EEnchantmentType.RemoteUnitAttributeChangeMP)
            {
                string onRemoteTriggerFilter = enchantmentInstance.source.Get().onRemoteTriggerFilter;
                if (customTarget != null && !string.IsNullOrEmpty(onRemoteTriggerFilter) && !(bool)ScriptLibrary.Call(onRemoteTriggerFilter, enchantable, enchantmentScript, enchantmentInstance, data))
                {
                    continue;
                }
            }
            ScriptLibrary.Call(enchantmentScript.script, enchantable, enchantmentScript, enchantmentInstance, data);
        }
        this.localActiveIterators--;
        this.IfAllIterationsFinished();
    }

    private ScriptIterator StartIteration(EEnchantmentType eType)
    {
        if (this.scripts != null && this.scripts.TryGetValue(eType, out var value))
        {
            return new ScriptIterator(eType, value);
        }
        return new ScriptIterator(eType, null);
    }

    public void ProcessIntigerScripts(EEnchantmentType eType, ref int value)
    {
        this.localActiveIterators++;
        ScriptIterator scriptIterator = this.StartIteration(eType);
        while (scriptIterator.Current() != null)
        {
            EnchantmentScript enchantmentScript = scriptIterator.Current();
            if (this.scriptToinstance.ContainsKey(enchantmentScript))
            {
                value = (int)ScriptLibrary.Call(enchantmentScript.script, this.owner, enchantmentScript, this.scriptToinstance[enchantmentScript], value);
            }
            scriptIterator.Next();
        }
        this.localActiveIterators--;
        this.IfAllIterationsFinished();
    }

    public void ProcessIntigerScripts(EEnchantmentType eType, ref int income, ref int upkeep)
    {
        this.localActiveIterators++;
        ScriptIterator scriptIterator = this.StartIteration(eType);
        while (scriptIterator.Current() != null)
        {
            EnchantmentScript enchantmentScript = scriptIterator.Current();
            if (this.scriptToinstance.ContainsKey(enchantmentScript))
            {
                int num = income + upkeep;
                int num2 = (int)ScriptLibrary.Call(enchantmentScript.script, this.owner, enchantmentScript, this.scriptToinstance[enchantmentScript], num) - num;
                if (num2 < 0)
                {
                    upkeep += num2;
                }
                else
                {
                    income += num2;
                }
            }
            scriptIterator.Next();
        }
        this.localActiveIterators--;
        this.IfAllIterationsFinished();
    }

    public void ProcessIntigerScripts(EEnchantmentType eType, ref int income, StatDetails details)
    {
        this.localActiveIterators++;
        ScriptIterator scriptIterator = this.StartIteration(eType);
        while (scriptIterator.Current() != null)
        {
            EnchantmentScript enchantmentScript = scriptIterator.Current();
            if (this.scriptToinstance.ContainsKey(enchantmentScript))
            {
                int num = income;
                income = (int)ScriptLibrary.Call(enchantmentScript.script, this.owner, enchantmentScript, this.scriptToinstance[enchantmentScript], income);
                int amount = income - num;
                EnchantmentInstance enchantmentInstance = this.scriptToinstance[enchantmentScript];
                details?.Add(enchantmentInstance.source.dbName, amount);
            }
            scriptIterator.Next();
        }
        this.localActiveIterators--;
        this.IfAllIterationsFinished();
    }

    public void ProcessFloatScripts(EEnchantmentType eType, ref float value)
    {
        this.localActiveIterators++;
        ScriptIterator scriptIterator = this.StartIteration(eType);
        while (scriptIterator.Current() != null)
        {
            EnchantmentScript enchantmentScript = scriptIterator.Current();
            if (this.scriptToinstance.ContainsKey(enchantmentScript))
            {
                value = (float)ScriptLibrary.Call(enchantmentScript.script, this.owner, enchantmentScript, this.scriptToinstance[enchantmentScript], value);
            }
            scriptIterator.Next();
        }
        this.localActiveIterators--;
        this.IfAllIterationsFinished();
    }

    public void ProcessFIntScripts(EEnchantmentType eType, ref FInt value)
    {
        this.localActiveIterators++;
        ScriptIterator scriptIterator = this.StartIteration(eType);
        while (scriptIterator.Current() != null)
        {
            EnchantmentScript enchantmentScript = scriptIterator.Current();
            if (this.scriptToinstance.ContainsKey(enchantmentScript))
            {
                value = (FInt)ScriptLibrary.Call(enchantmentScript.script, this.owner, enchantmentScript, this.scriptToinstance[enchantmentScript], value);
            }
            scriptIterator.Next();
        }
        this.localActiveIterators--;
        this.IfAllIterationsFinished();
    }

    public List<EnchantmentInstance> GetEnchantments()
    {
        return this.enchantments;
    }

    public HashSet<EnchantmentInstance> GetRemoteEnchantments(IEnchantable target)
    {
        HashSet<EnchantmentInstance> hashSet = null;
        NetDictionary<DBReference<Tag>, FInt> netDictionary = null;
        if (target is IAttributable)
        {
            netDictionary = (target as IAttributable).GetAttributes().finalAttributes;
        }
        if (this.scripts.ContainsKey(EEnchantmentType.RemoteUnitAttributeChange))
        {
            foreach (EnchantmentScript item in this.scripts[EEnchantmentType.RemoteUnitAttributeChange])
            {
                EnchantmentInstance enchantmentInstance = this.scriptToinstance[item];
                if (string.IsNullOrEmpty(enchantmentInstance.source.Get().onRemoteTriggerFilter) || (bool)ScriptLibrary.Call(enchantmentInstance.source.Get().onRemoteTriggerFilter, target, item, enchantmentInstance, netDictionary))
                {
                    if (hashSet == null)
                    {
                        hashSet = new HashSet<EnchantmentInstance>();
                    }
                    hashSet.Add(enchantmentInstance);
                }
            }
        }
        if (this.scripts.ContainsKey(EEnchantmentType.RemoteUnitAttributeChangeMP))
        {
            foreach (EnchantmentScript item2 in this.scripts[EEnchantmentType.RemoteUnitAttributeChangeMP])
            {
                EnchantmentInstance enchantmentInstance2 = this.scriptToinstance[item2];
                if (string.IsNullOrEmpty(enchantmentInstance2.source.Get().onRemoteTriggerFilter) || (bool)ScriptLibrary.Call(enchantmentInstance2.source.Get().onRemoteTriggerFilter, target, item2, enchantmentInstance2, netDictionary))
                {
                    if (hashSet == null)
                    {
                        hashSet = new HashSet<EnchantmentInstance>();
                    }
                    hashSet.Add(enchantmentInstance2);
                }
            }
        }
        return hashSet;
    }

    public bool ContainsType(EEnchantmentType eType)
    {
        if (this.scripts == null)
        {
            return false;
        }
        if (this.scripts.ContainsKey(eType))
        {
            return true;
        }
        return false;
    }

    public List<EnchantmentInstance> GetEnchantmentsOfType(EEnchantmentType eType)
    {
        if (this.scripts == null || !this.scripts.ContainsKey(eType))
        {
            return null;
        }
        return this.enchantments.FindAll((EnchantmentInstance o) => o.source.Get().scripts != null && Array.Exists(o.source.Get().scripts, (EnchantmentScript et) => et.triggerType == eType));
    }

    public List<EnchantmentInstance> GetEnchantmentsWithRemotes(bool visibleOnly = false)
    {
        IEnchantable enchantable = this.owner;
        List<EnchantmentInstance> list = this.GetEnchantments();
        List<EnchantmentInstance> list2 = ((list != null) ? new List<EnchantmentInstance>(list) : new List<EnchantmentInstance>());
        BaseUnit baseUnit = enchantable as BaseUnit;
        if (baseUnit is global::MOM.Unit)
        {
            global::MOM.Unit unit = baseUnit as global::MOM.Unit;
            PlayerWizard wizardOwner = unit.GetWizardOwner();
            if (wizardOwner != null)
            {
                HashSet<EnchantmentInstance> remoteEnchantments = wizardOwner.GetRemoteEnchantments(unit);
                if (remoteEnchantments != null)
                {
                    list2.AddRange(remoteEnchantments);
                }
            }
            HashSet<EnchantmentInstance> remoteEnchantments2 = GameManager.Get().GetRemoteEnchantments(unit);
            if (remoteEnchantments2 != null)
            {
                list2.AddRange(remoteEnchantments2);
            }
            if (unit.group != null)
            {
                global::MOM.Location locationHostSmart = unit.group.Get().GetLocationHostSmart();
                if (locationHostSmart != null)
                {
                    HashSet<EnchantmentInstance> remoteEnchantments3 = locationHostSmart.GetRemoteEnchantments(unit);
                    if (remoteEnchantments3 != null)
                    {
                        list2.AddRange(remoteEnchantments3);
                    }
                }
            }
        }
        if (baseUnit is BattleUnit battleUnit)
        {
            Battle battle = Battle.GetBattle();
            if (battle != null)
            {
                HashSet<EnchantmentInstance> remoteEnchantments4 = battle.GetRemoteEnchantments(baseUnit);
                if (remoteEnchantments4 != null)
                {
                    list2.AddRange(remoteEnchantments4);
                }
                BattlePlayer player = battle.GetPlayer(battleUnit.attackingSide);
                if (player != null)
                {
                    remoteEnchantments4 = player.GetRemoteEnchantments(baseUnit);
                    if (remoteEnchantments4 != null)
                    {
                        list2.AddRange(remoteEnchantments4);
                    }
                }
            }
        }
        if (visibleOnly)
        {
            list2 = list2.FindAll((EnchantmentInstance o) => !o.source.Get().hideEnch);
        }
        return list2;
    }

    public void CountedownUpdate(IEnchantable obj)
    {
        if (this.enchantments == null || this.enchantments.Count <= 0)
        {
            return;
        }
        for (int num = this.enchantments.Count - 1; num >= 0; num--)
        {
            if (!this.enchantments[num].battleEnchantment)
            {
                if (this.enchantments[num].countDown > 0)
                {
                    this.enchantments[num].countDown--;
                }
                if (this.enchantments[num].countDown == 0)
                {
                    obj.RemoveEnchantment(this.enchantments[num].source.Get());
                }
            }
        }
    }

    public void BattleCountdownUpdate(IEnchantable obj, bool isAttackerTurn)
    {
        if (this.enchantments == null || this.enchantments.Count <= 0)
        {
            return;
        }
        for (int num = this.enchantments.Count - 1; num >= 0; num--)
        {
            EnchantmentInstance enchantmentInstance = this.enchantments[num];
            if (enchantmentInstance.battleEnchantment && enchantmentInstance.countDown > -1)
            {
                bool flag = this.owner.GetWizardOwner()?.GetID() == Battle.Get().attacker.GetID();
                if ((isAttackerTurn && flag) || (!isAttackerTurn && !flag))
                {
                    if (enchantmentInstance.countDown > 0)
                    {
                        enchantmentInstance.countDown--;
                    }
                    if (enchantmentInstance.countDown == 0)
                    {
                        obj.RemoveEnchantment(enchantmentInstance.source.Get());
                    }
                }
            }
        }
    }

    public EnchantmentManager CopyEnchantmentManager(IEnchantable newOwner)
    {
        EnchantmentManager enchantmentManager = new EnchantmentManager();
        enchantmentManager.owner = newOwner;
        foreach (KeyValuePair<EEnchantmentType, List<EnchantmentScript>> script in this.scripts)
        {
            enchantmentManager.scripts[script.Key] = new List<EnchantmentScript>(script.Value);
        }
        enchantmentManager.enchantments = new List<EnchantmentInstance>(this.enchantments);
        enchantmentManager.scriptToinstance = new Dictionary<EnchantmentScript, EnchantmentInstance>(this.scriptToinstance);
        enchantmentManager.onJoinTriggers = ((this.onJoinTriggers != null) ? new List<EnchantmentInstance>(this.onJoinTriggers) : null);
        enchantmentManager.onLeaveTriggers = ((this.onLeaveTriggers != null) ? new List<EnchantmentInstance>(this.onLeaveTriggers) : null);
        return enchantmentManager;
    }

    public void CopyEnchantmentManagerFrom(IEnchantable source, bool useRequirementScripts = false)
    {
        foreach (EnchantmentInstance ei in source.GetEnchantmentManager().GetEnchantments())
        {
            if (this.GetEnchantments().Find((EnchantmentInstance o) => o.source == ei.source) != null)
            {
                continue;
            }
            if (useRequirementScripts)
            {
                EnchantmentScript requirementScript = ei.source.Get().requirementScript;
                if (requirementScript != null && (bool)ScriptLibrary.Call(requirementScript.script, this.owner))
                {
                    Enchantment enchantment = ei.source;
                    EnchantmentInstance enchantmentInstance = new EnchantmentInstance();
                    enchantmentInstance.nameID = enchantment.dbName;
                    enchantmentInstance.countDown = ei.countDown;
                    enchantmentInstance.manager = this;
                    enchantmentInstance.owner = ei.owner;
                    enchantmentInstance.source = enchantment;
                    enchantmentInstance.parameters = ei.parameters;
                    enchantmentInstance.battleEnchantment = ei.battleEnchantment;
                    enchantmentInstance.dispelCost = ei.dispelCost;
                    enchantmentInstance.upkeepMana = enchantment.upkeepCost;
                    this.owner.AddEnchantment(enchantmentInstance);
                }
            }
            else
            {
                Enchantment enchantment2 = ei.source;
                EnchantmentInstance enchantmentInstance2 = new EnchantmentInstance();
                enchantmentInstance2.nameID = enchantment2.dbName;
                enchantmentInstance2.countDown = ei.countDown;
                enchantmentInstance2.manager = this;
                enchantmentInstance2.owner = ei.owner;
                enchantmentInstance2.source = enchantment2;
                enchantmentInstance2.parameters = ei.parameters;
                enchantmentInstance2.battleEnchantment = ei.battleEnchantment;
                enchantmentInstance2.dispelCost = ei.dispelCost;
                enchantmentInstance2.upkeepMana = enchantment2.upkeepCost;
                this.owner.AddEnchantment(enchantmentInstance2);
            }
        }
    }

    private void SetEnchantmentManagers()
    {
        foreach (EnchantmentInstance enchantment in this.enchantments)
        {
            enchantment.manager = this;
        }
    }

    private void EnsureDictionaries()
    {
        foreach (EnchantmentInstance enchantment in this.enchantments)
        {
            if (enchantment.source.Get().scripts == null)
            {
                continue;
            }
            EnchantmentScript[] array = enchantment.source.Get().scripts;
            foreach (EnchantmentScript enchantmentScript in array)
            {
                if (!this.scripts.ContainsKey(enchantmentScript.triggerType))
                {
                    this.scripts[enchantmentScript.triggerType] = new List<EnchantmentScript>();
                }
                this.scripts[enchantmentScript.triggerType].Add(enchantmentScript);
                this.scriptToinstance[enchantmentScript] = enchantment;
            }
        }
    }

    private void EnsureJoinLeaveLists()
    {
        foreach (EnchantmentInstance enchantment in this.enchantments)
        {
            if (!string.IsNullOrEmpty(enchantment.source.Get().onJoinWithUnit))
            {
                if (this.onJoinTriggers == null)
                {
                    this.onJoinTriggers = new List<EnchantmentInstance>();
                }
                this.onJoinTriggers.Add(enchantment);
            }
            if (!string.IsNullOrEmpty(enchantment.source.Get().onLeaveFromUnit))
            {
                if (this.onLeaveTriggers == null)
                {
                    this.onLeaveTriggers = new List<EnchantmentInstance>();
                }
                this.onLeaveTriggers.Add(enchantment);
            }
        }
    }

    public void Destroy()
    {
        for (int num = this.enchantments.Count - 1; num >= 0; num--)
        {
            if (this.enchantments[num] != null)
            {
                this.Remove(this.enchantments[num]);
            }
        }
    }

    public void OnJoinTriggers(IEnchantable otherEnchantable, IEnumerable unitList)
    {
        if (this.onJoinTriggers == null)
        {
            return;
        }
        List<BaseUnit> list = null;
        if (unitList != null)
        {
            list = new List<BaseUnit>();
            foreach (object unit in unitList)
            {
                if (unit is Reference<global::MOM.Unit>)
                {
                    list.Add((unit as Reference<global::MOM.Unit>).Get());
                }
                else if (unit is BattleUnit)
                {
                    list.Add(unit as BattleUnit);
                }
            }
        }
        foreach (EnchantmentInstance onJoinTrigger in this.onJoinTriggers)
        {
            ScriptLibrary.Call(onJoinTrigger.source.Get().onJoinWithUnit, this.owner, otherEnchantable, onJoinTrigger, list);
        }
    }

    public void OnLeaveTriggers(IEnchantable otherEnchantable, IEnumerable unitList)
    {
        if (this.onLeaveTriggers == null)
        {
            return;
        }
        List<BaseUnit> list = null;
        if (unitList != null)
        {
            list = new List<BaseUnit>();
            foreach (object unit in unitList)
            {
                if (unit is Reference<global::MOM.Unit>)
                {
                    list.Add((unit as Reference<global::MOM.Unit>).Get());
                }
                else if (unit is BattleUnit)
                {
                    list.Add(unit as BattleUnit);
                }
            }
        }
        foreach (EnchantmentInstance onLeaveTrigger in this.onLeaveTriggers)
        {
            ScriptLibrary.Call(onLeaveTrigger.source.Get().onLeaveFromUnit, this.owner, otherEnchantable, onLeaveTrigger, list);
        }
    }

    public void EnsureEnchantments()
    {
        for (int num = this.enchantments.Count - 1; num >= 0; num--)
        {
            bool flag = true;
            EnchantmentScript requirementScript = this.enchantments[num].source.Get().requirementScript;
            if (requirementScript != null)
            {
                flag = (bool)ScriptLibrary.Call(requirementScript.script, this.owner);
            }
            if (!flag)
            {
                this.owner.RemoveEnchantment(this.enchantments[num]);
            }
        }
    }

    private void IfAllIterationsFinished()
    {
        if (this.localActiveIterators == 0)
        {
            this.owner?.FinishedIteratingEnchantments();
        }
    }
}
