using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DBDef;
using MHUtils;
using MHUtils.UI;
using ProtoBuf;
using UnityEngine;

namespace MOM
{
    [ProtoContract]
    public class EnchantmentManager
    {
        private class ScriptIterator
        {
            private static List<List<ScriptIterator>> currentIterators;

            private EEnchantmentType eEnchType;

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

            public static bool ItemRemoved(EEnchantmentType eEnchType, int index, List<EnchantmentScript> list)
            {
                List<ScriptIterator> obj = ScriptIterator.currentIterators[(int)eEnchType];
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

            public ScriptIterator(EEnchantmentType eEnchType, List<EnchantmentScript> list)
            {
                this.eEnchType = eEnchType;
                this.list = list;
                current = ((list != null) ? (list.Count - 1) : (-1));
                next = current - 1;
                if (current >= 0)
                {
                    ScriptIterator.currentIterators[(int)eEnchType].Add(this);
                }
            }

            public EnchantmentScript Current()
            {
                if (current >= 0)
                {
                    return list[current];
                }
                return null;
            }

            public void Next()
            {
                current = next;
                next--;
                if (current < 0)
                {
                    ScriptIterator.currentIterators[(int)eEnchType].Remove(this);
                    list = null;
                }
            }

            private void InternalItemRemoved(int index)
            {
                if (current == index)
                {
                    current = -1;
                }
                else if (current > index)
                {
                    current--;
                }
                if (next >= index)
                {
                    next--;
                }
                if (next < 0 && current < 0)
                {
                    ScriptIterator.currentIterators[(int)eEnchType].Remove(this);
                    list = null;
                }
            }

            public void End()
            {
                ScriptIterator.currentIterators[(int)eEnchType].Remove(this);
                next = -1;
                current = -1;
                list = null;
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
            if (!EnchantmentRegister.IsInstantiated() || (owner is BattleUnit || (owner is BattlePlayer || (owner is Battle || enchantments == null))))
            {
                return;
            }
            foreach (EnchantmentInstance ei in enchantments)
            {
                EnchantmentRegister.EnchantmentRemoved(ei);
            }
        }

        [ProtoAfterDeserialization]
        public void Reinitialization()
        {
            EnsureDictionaries();
            EnsureJoinLeaveLists();
            SetEnchantmentManagers();
        }

        public EnchantmentInstance Add(Enchantment effect, Entity owner = null, int countDown = -1, string parameters = null, bool inBattle = false, int dispelcost = 0)
        {
            iteration++;
            EnchantmentInstance ei = new EnchantmentInstance();
            ei.nameID = effect.dbName;
            ei.countDown = countDown;
            ei.manager = this;
            ei.owner = ((owner != null) ? new Reference(owner) : null);
            ei.source = effect;
            ei.parameters = parameters;
            ei.battleEnchantment = inBattle;
            ei.dispelCost = dispelcost;
            ei.upkeepMana = effect.upkeepCost;
            enchantments.Add(ei);
            if (effect.scripts != null)
            {
                EnchantmentScript[] array = effect.scripts;
                foreach (EnchantmentScript enchantmentScript in array)
                {
                    if (!scripts.ContainsKey(enchantmentScript.triggerType))
                    {
                        scripts[enchantmentScript.triggerType] = new List<EnchantmentScript>();
                    }
                    scripts[enchantmentScript.triggerType].Add(enchantmentScript);
                    ei.enchantmentHolder = ((this.owner.GetWizardOwner() != null) ? this.owner.GetWizardOwner().ID : 0);
                    scriptToinstance[enchantmentScript] = ei;
                    if (enchantmentScript.triggerType == EEnchantmentType.VisibilityRangeModifier && owner is IPlanePosition planePosition)
                    {
                        FOW.Get().UpdateFogForPlane(planePosition.GetPlane());
                    }
                }
            }
            if (!string.IsNullOrEmpty(effect.onJoinWithUnit))
            {
                if (onJoinTriggers == null)
                {
                    onJoinTriggers = new List<EnchantmentInstance>();
                }
                onJoinTriggers.Add(ei);
            }
            if (!string.IsNullOrEmpty(effect.onLeaveFromUnit))
            {
                if (onLeaveTriggers == null)
                {
                    onLeaveTriggers = new List<EnchantmentInstance>();
                }
                onLeaveTriggers.Add(ei);
            }
            if (owner is TownLocation)
            {
                VerticalMarkerManager.Get().UpdateInfoOnMarker(owner);
                (owner as TownLocation).UpdateOwnerModels();
            }
            if (owner is BattleUnit)
            {
                BattleHUD.Get()?.Dirty();
            }
            else if (!inBattle)
            {
                EnchantmentRegister.EnchantmentAdded(ei);
            }
            return ei;
        }

        public EnchantmentInstance Add2(EnchantmentInstance ei, bool inBattle = false)
        {
            iteration++;
            ei.battleEnchantment = inBattle;
            ei.manager = this;
            enchantments.Add(ei);
            if (ei.source.Get().scripts != null)
            {
                EnchantmentScript[] array = ei.source.Get().scripts;
                foreach (EnchantmentScript es in array)
                {
                    if (!scripts.ContainsKey(es.triggerType))
                    {
                        scripts[es.triggerType] = new List<EnchantmentScript>();
                    }
                    scripts[es.triggerType].Add(es);
                    scriptToinstance[es] = ei;
                }
            }
            if (!string.IsNullOrEmpty(ei.source.Get().onJoinWithUnit))
            {
                if (onJoinTriggers == null)
                {
                    onJoinTriggers = new List<EnchantmentInstance>();
                }
                onJoinTriggers.Add(ei);
            }
            if (!string.IsNullOrEmpty(ei.source.Get().onLeaveFromUnit))
            {
                if (onLeaveTriggers == null)
                {
                    onLeaveTriggers = new List<EnchantmentInstance>();
                }
                onLeaveTriggers.Add(ei);
            }
            if (owner is TownLocation townLocation)
            {
                VerticalMarkerManager.Get().UpdateInfoOnMarker(owner);
                townLocation.EnchantmentsChanged();
            }
            if (owner is Battle || owner is BattlePlayer || owner is BattleUnit)
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
            iteration++;
            for (int i = 0; i < enchantments.Count; i++)
            {
                if (enchantments[i].source.Get() == effect)
                {
                    MHEventSystem.TriggerEvent("RemoveEnchantment", this, enchantments[i]);
                    return Remove(enchantments[i]);
                }
            }
            if (owner is TownLocation townLocation)
            {
                townLocation.EnchantmentsChanged();
            }
            if (owner is Battle || owner is BattlePlayer || owner is BattleUnit)
            {
                BattleHUD.Get()?.Dirty();
            }
            return null;
        }

        public EnchantmentInstance Remove(EnchantmentInstance ei)
        {
            iteration++;
            while (true)
            {
                EnchantmentScript enchantmentScript = null;
                foreach (KeyValuePair<EnchantmentScript, EnchantmentInstance> item in scriptToinstance)
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
                scriptToinstance.Remove(enchantmentScript);
                EEnchantmentType triggerType = enchantmentScript.triggerType;
                if (!scripts.TryGetValue(triggerType, out List<EnchantmentScript> value) || value == null)
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
            enchantments.Remove(ei);
            if (!string.IsNullOrEmpty(ei.source.Get().onJoinWithUnit) && onJoinTriggers != null)
            {
                onJoinTriggers.Remove(ei);
                if (onJoinTriggers.Count == 0)
                {
                    onJoinTriggers = null;
                }
            }
            if (!string.IsNullOrEmpty(ei.source.Get().onLeaveFromUnit) && onLeaveTriggers != null)
            {
                onLeaveTriggers.Remove(ei);
                if (onLeaveTriggers.Count == 0)
                {
                    onLeaveTriggers = null;
                }
            }
            if (owner is TownLocation townLocation)
            {
                VerticalMarkerManager.Get().UpdateInfoOnMarker(owner);
                townLocation.EnchantmentsChanged();
            }
            EnchantmentRegister.EnchantmentRemoved(ei);
            if (owner is Battle || owner is BattlePlayer || owner is BattleUnit)
            {
                BattleHUD.Get()?.Dirty();
            }
            return ei;
        }

        public void TriggerScripts(EEnchantmentType eEnchType, object data, IEnchantable customTarget = null)
        {
            if (eEnchType == EEnchantmentType.BattleEndEffect)
                {
                 Debug.Log("enter TriggerScripts-BattleEndEffect");
                }

            if (scripts == null || !scripts.ContainsKey(eEnchType))
            {
                return;
            }
            IEnchantable enchantable = customTarget;
            if (enchantable == null)
            {
                enchantable = owner;
            }
            localActiveIterators++;
            for (ScriptIterator si = StartIteration(eEnchType); si.Current() != null; si.Next())
            {
                EnchantmentScript es = si.Current();
                if (es == null)
                {
                    Debug.LogError("null script in the dictionary of type : " + eEnchType);
                }
                if (!scriptToinstance.ContainsKey(es))
                {
                    continue;
                }
                EnchantmentInstance ei = scriptToinstance[es];
                if (eEnchType == EEnchantmentType.RemoteUnitAttributeChange || eEnchType == EEnchantmentType.RemoteUnitAttributeChangeMP)
                {
                    string onRemoteTriggerFilter = ei.source.Get().onRemoteTriggerFilter;
                    if (customTarget != null && 
                        !string.IsNullOrEmpty(onRemoteTriggerFilter) && 
                        !(bool)ScriptLibrary.Call(onRemoteTriggerFilter, enchantable, es, ei, data))
                    {
                        continue;
                    }
                }

                if (eEnchType == EEnchantmentType.BattleEndEffect)
                {
                   Debug.Log("TriggerScripts-BattleEndEffect calling ScriptLibrary : " + es.script);
                }
                ScriptLibrary.Call(es.script, enchantable, es, ei, data);
            }
            localActiveIterators--;
            IfAllIterationsFinished();
        }

        private ScriptIterator StartIteration(EEnchantmentType eEnchType)
        {
            if (scripts != null && scripts.TryGetValue(eEnchType, out List<EnchantmentScript> value))
            {
                return new ScriptIterator(eEnchType, value);
            }
            return new ScriptIterator(eEnchType, null);
        }

        public void ProcessIntigerScripts(EEnchantmentType eEnchType, ref int value)
        {
            localActiveIterators++;
            ScriptIterator scriptIterator = StartIteration(eEnchType);
            while (scriptIterator.Current() != null)
            {
                EnchantmentScript es = scriptIterator.Current();
                if (scriptToinstance.ContainsKey(es))
                {
                    value = (int)ScriptLibrary.Call(es.script, owner, es, scriptToinstance[es], value);
                }
                scriptIterator.Next();
            }
            localActiveIterators--;
            IfAllIterationsFinished();
        }

        public void ProcessIntigerScripts(EEnchantmentType eEnchType, ref int income, ref int upkeep)
        {
            localActiveIterators++;
            ScriptIterator scriptIterator = StartIteration(eEnchType);
            while (scriptIterator.Current() != null)
            {
                EnchantmentScript es = scriptIterator.Current();
                if (scriptToinstance.ContainsKey(es))
                {
                    int num = income + upkeep;
                    int num2 = (int)ScriptLibrary.Call(es.script, owner, es, scriptToinstance[es], num) - num;
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
            localActiveIterators--;
            IfAllIterationsFinished();
        }

        public void ProcessIntigerScripts(EEnchantmentType eEnchType, ref int income, StatDetails details)
        {
            localActiveIterators++;
            ScriptIterator scriptIterator = StartIteration(eEnchType);
            while (scriptIterator.Current() != null)
            {
                EnchantmentScript es = scriptIterator.Current();
                if (scriptToinstance.ContainsKey(es))
                {
                    int num = income;
                    income = (int)ScriptLibrary.Call(es.script, owner, es, scriptToinstance[es], income);
                    int amount = income - num;
                    EnchantmentInstance ei = scriptToinstance[es];
                    details?.Add(ei.source.dbName, amount);
                }
                scriptIterator.Next();
            }
            localActiveIterators--;
            IfAllIterationsFinished();
        }

        public void ProcessFloatScripts(EEnchantmentType eEnchType, ref float value)
        {
            localActiveIterators++;
            ScriptIterator scriptIterator = StartIteration(eEnchType);
            while (scriptIterator.Current() != null)
            {
                EnchantmentScript es = scriptIterator.Current();
                if (scriptToinstance.ContainsKey(es))
                {
                    value = (float)ScriptLibrary.Call(es.script, owner, es, scriptToinstance[es], value);
                }
                scriptIterator.Next();
            }
            localActiveIterators--;
            IfAllIterationsFinished();
        }

        public void ProcessFIntScripts(EEnchantmentType eEnchType, ref FInt value)
        {
            localActiveIterators++;
            ScriptIterator scriptIterator = StartIteration(eEnchType);
            while (scriptIterator.Current() != null)
            {
                EnchantmentScript es = scriptIterator.Current();
                if (scriptToinstance.ContainsKey(es))
                {
                    value = (FInt)ScriptLibrary.Call(es.script, owner, es, scriptToinstance[es], value);
                }
                scriptIterator.Next();
            }
            localActiveIterators--;
            IfAllIterationsFinished();
        }

        public List<EnchantmentInstance> GetEnchantments()
        {
            return enchantments;
        }

        public HashSet<EnchantmentInstance> GetRemoteEnchantments(IEnchantable target)
        {
            HashSet<EnchantmentInstance> hashSet = null;
            NetDictionary<DBReference<Tag>, FInt> netDictionary = null;
            if (target is IAttributable)
            {
                netDictionary = (target as IAttributable).GetAttributes().finalAttributes;
            }
            if (scripts.ContainsKey(EEnchantmentType.RemoteUnitAttributeChange))
            {
                foreach (EnchantmentScript es in scripts[EEnchantmentType.RemoteUnitAttributeChange])
                {
                    EnchantmentInstance ei = scriptToinstance[es];
                    if (string.IsNullOrEmpty(ei.source.Get().onRemoteTriggerFilter) || 
                        (bool)ScriptLibrary.Call(ei.source.Get().onRemoteTriggerFilter, target, es, ei, netDictionary))
                    {
                        if (hashSet == null)
                        {
                            hashSet = new HashSet<EnchantmentInstance>();
                        }
                        hashSet.Add(ei);
                    }
                }
            }
            if (scripts.ContainsKey(EEnchantmentType.RemoteUnitAttributeChangeMP))
            {
                foreach (EnchantmentScript es2 in scripts[EEnchantmentType.RemoteUnitAttributeChangeMP])
                {
                    EnchantmentInstance ei2 = scriptToinstance[es2];
                    if (string.IsNullOrEmpty(ei2.source.Get().onRemoteTriggerFilter) || 
                        (bool)ScriptLibrary.Call(ei2.source.Get().onRemoteTriggerFilter, target, es2, ei2, netDictionary))
                    {
                        if (hashSet == null)
                        {
                            hashSet = new HashSet<EnchantmentInstance>();
                        }
                        hashSet.Add(ei2);
                    }
                }
            }
            return hashSet;
        }

        public bool ContainsType(EEnchantmentType eEnchType)
        {
            if (scripts == null)
            {
                return false;
            }
            if (scripts.ContainsKey(eEnchType))
            {
                return true;
            }
            return false;
        }

        public List<EnchantmentInstance> GetEnchantmentsOfType(EEnchantmentType eEnchType)
        {
            if (scripts == null || !scripts.ContainsKey(eEnchType))
            {
                return null;
            }
            return enchantments.FindAll((EnchantmentInstance o) => o.source.Get().scripts != null && Array.Exists(o.source.Get().scripts, (EnchantmentScript et) => et.triggerType == eEnchType));
        }

        public List<EnchantmentInstance> GetEnchantmentsWithRemotes(bool visibleOnly = false)
        {
            IEnchantable enchantable = owner;
            List<EnchantmentInstance> list = GetEnchantments();
            List<EnchantmentInstance> list2 = ((list != null) ? new List<EnchantmentInstance>(list) : new List<EnchantmentInstance>());
            BaseUnit baseUnit = enchantable as BaseUnit;
            if (baseUnit is Unit)
            {
                Unit unit = baseUnit as Unit;
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
                    Location locationHostSmart = unit.group.Get().GetLocationHostSmart();
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
            if (enchantments == null || enchantments.Count <= 0)
            {
                return;
            }
            for (int num = enchantments.Count - 1; num >= 0; num--)
            {
                if (!enchantments[num].battleEnchantment)
                {
                    if (enchantments[num].countDown > 0)
                    {
                        enchantments[num].countDown--;
                    }
                    if (enchantments[num].countDown == 0)
                    {
                        obj.RemoveEnchantment(enchantments[num].source.Get());
                    }
                }
            }
        }

        public void BattleCountdownUpdate(IEnchantable obj, bool isAttackerTurn)
        {
            if (enchantments == null || enchantments.Count <= 0)
            {
                return;
            }
            for (int num = enchantments.Count - 1; num >= 0; num--)
            {
                EnchantmentInstance ei = enchantments[num];
                if (ei.battleEnchantment && ei.countDown > -1)
                {
                    bool flag = owner.GetWizardOwner()?.GetID() == Battle.Get().attacker.GetID();
                    if ((isAttackerTurn && flag) || (!isAttackerTurn && !flag))
                    {
                        if (ei.countDown > 0)
                        {
                            ei.countDown--;
                        }
                        if (ei.countDown == 0)
                        {
                            obj.RemoveEnchantment(ei.source.Get());
                        }
                    }
                }
            }
        }

        public EnchantmentManager CopyEnchantmentManager(IEnchantable newOwner)
        {
            EnchantmentManager enchantmentManager = new EnchantmentManager();
            enchantmentManager.owner = newOwner;
            foreach (KeyValuePair<EEnchantmentType, List<EnchantmentScript>> script in scripts)
            {
                enchantmentManager.scripts[script.Key] = new List<EnchantmentScript>(script.Value);
            }
            enchantmentManager.enchantments = new List<EnchantmentInstance>(enchantments);
            enchantmentManager.scriptToinstance = new Dictionary<EnchantmentScript, EnchantmentInstance>(scriptToinstance);
            enchantmentManager.onJoinTriggers = ((onJoinTriggers != null) ? new List<EnchantmentInstance>(onJoinTriggers) : null);
            enchantmentManager.onLeaveTriggers = ((onLeaveTriggers != null) ? new List<EnchantmentInstance>(onLeaveTriggers) : null);
            return enchantmentManager;
        }

        public void CopyEnchantmentManagerFrom(IEnchantable source, bool useRequirementScripts = false)
        {
            foreach (EnchantmentInstance ei in source.GetEnchantmentManager().GetEnchantments())
            {
                if (GetEnchantments().Find((EnchantmentInstance o) => o.source == ei.source) != null)
                {
                    continue;
                }
                if (useRequirementScripts)
                {
                    EnchantmentScript es = ei.source.Get().requirementScript;
                    if (es != null && (bool)ScriptLibrary.Call(es.script, owner))
                    {
                        Enchantment enchantment = ei.source;
                        EnchantmentInstance ei2 = new EnchantmentInstance();
                        ei2.nameID = enchantment.dbName;
                        ei2.countDown = ei.countDown;
                        ei2.manager = this;
                        ei2.owner = ei.owner;
                        ei2.source = enchantment;
                        ei2.parameters = ei.parameters;
                        ei2.battleEnchantment = ei.battleEnchantment;
                        ei2.dispelCost = ei.dispelCost;
                        ei2.upkeepMana = enchantment.upkeepCost;
                        owner.AddEnchantment(ei2);
                    }
                }
                else
                {
                    Enchantment enchantment2 = ei.source;
                    EnchantmentInstance ei3 = new EnchantmentInstance();
                    ei3.nameID = enchantment2.dbName;
                    ei3.countDown = ei.countDown;
                    ei3.manager = this;
                    ei3.owner = ei.owner;
                    ei3.source = enchantment2;
                    ei3.parameters = ei.parameters;
                    ei3.battleEnchantment = ei.battleEnchantment;
                    ei3.dispelCost = ei.dispelCost;
                    ei3.upkeepMana = enchantment2.upkeepCost;
                    owner.AddEnchantment(ei3);
                }
            }
        }

        private void SetEnchantmentManagers()
        {
            foreach (EnchantmentInstance ei in enchantments)
            {
                ei.manager = this;
            }
        }

        private void EnsureDictionaries()
        {
            foreach (EnchantmentInstance ei in enchantments)
            {
                if (ei.source.Get().scripts == null)
                {
                    continue;
                }
                EnchantmentScript[] array = ei.source.Get().scripts;
                foreach (EnchantmentScript es in array)
                {
                    if (!scripts.ContainsKey(es.triggerType))
                    {
                        scripts[es.triggerType] = new List<EnchantmentScript>();
                    }
                    scripts[es.triggerType].Add(es);
                    scriptToinstance[es] = ei;
                }
            }
        }

        private void EnsureJoinLeaveLists()
        {
            foreach (EnchantmentInstance ei in enchantments)
            {
                if (!string.IsNullOrEmpty(ei.source.Get().onJoinWithUnit))
                {
                    if (onJoinTriggers == null)
                    {
                        onJoinTriggers = new List<EnchantmentInstance>();
                    }
                    onJoinTriggers.Add(ei);
                }
                if (!string.IsNullOrEmpty(ei.source.Get().onLeaveFromUnit))
                {
                    if (onLeaveTriggers == null)
                    {
                        onLeaveTriggers = new List<EnchantmentInstance>();
                    }
                    onLeaveTriggers.Add(ei);
                }
            }
        }

        public void Destroy()
        {
            for (int num = enchantments.Count - 1; num >= 0; num--)
            {
                if (enchantments[num] != null)
                {
                    Remove(enchantments[num]);
                }
            }
        }

        public void OnJoinTriggers(IEnchantable otherEnchantable, IEnumerable unitList)
        {
            if (onJoinTriggers == null)
            {
                return;
            }
            List<BaseUnit> list = null;
            if (unitList != null)
            {
                list = new List<BaseUnit>();
                foreach (object unit in unitList)
                {
                    if (unit is Reference<Unit>)
                    {
                        list.Add((unit as Reference<Unit>).Get());
                    }
                    else if (unit is BattleUnit)
                    {
                        list.Add(unit as BattleUnit);
                    }
                }
            }
            foreach (EnchantmentInstance ei in onJoinTriggers)
            {
                ScriptLibrary.Call(ei.source.Get().onJoinWithUnit, owner, otherEnchantable, ei, list);
            }
        }

        public void OnLeaveTriggers(IEnchantable otherEnchantable, IEnumerable unitList)
        {
            if (onLeaveTriggers == null)
            {
                return;
            }
            List<BaseUnit> list = null;
            if (unitList != null)
            {
                list = new List<BaseUnit>();
                foreach (object unit in unitList)
                {
                    if (unit is Reference<Unit>)
                    {
                        list.Add((unit as Reference<Unit>).Get());
                    }
                    else if (unit is BattleUnit)
                    {
                        list.Add(unit as BattleUnit);
                    }
                }
            }
            foreach (EnchantmentInstance ei in onLeaveTriggers)
            {
                ScriptLibrary.Call(ei.source.Get().onLeaveFromUnit, owner, otherEnchantable, ei, list);
            }
        }

        public void EnsureEnchantments()
        {
            for (int num = enchantments.Count - 1; num >= 0; num--)
            {
                bool flag = true;
                EnchantmentScript es = enchantments[num].source.Get().requirementScript;
                if (es != null)
                {
                    flag = (bool)ScriptLibrary.Call(es.script, owner);
                }
                if (!flag)
                {
                    owner.RemoveEnchantment(enchantments[num]);
                }
            }
        }

        private void IfAllIterationsFinished()
        {
            if (localActiveIterators == 0)
            {
                owner?.FinishedIteratingEnchantments();
            }
        }
    }
}
