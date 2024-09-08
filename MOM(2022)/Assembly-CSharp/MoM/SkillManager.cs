using System.Collections;
using System.Collections.Generic;
using DBDef;
using ProtoBuf;
using UnityEngine;

namespace MOM
{
    [ProtoContract]
    public class SkillManager
    {
        [ProtoIgnore]
        public ISkillable owner;

        [ProtoIgnore]
        public Dictionary<ESkillType, List<SkillScript>> scripts;

        [ProtoIgnore]
        public List<Skill> onJoinTriggers;

        [ProtoIgnore]
        public List<Skill> onLeaveTriggers;

        [ProtoMember(1)]
        public List<DBReference<Skill>> skills = new List<DBReference<Skill>>();

        public SkillManager()
        {
        }

        public SkillManager(ISkillable owner)
        {
            this.owner = owner;
            this.EnsureScriptDictionary();
        }

        [ProtoAfterDeserialization]
        public void Reinitialization()
        {
            this.EnsureScriptDictionary();
            this.EnsureJoinLeaveLists();
        }

        public void Add(Skill skill)
        {
            if (skill == null)
            {
                Debug.LogWarning("skill is null");
                return;
            }
            if (skill.script == null && skill.applicationScript == null && skill.removalScript == null)
            {
                Debug.LogWarning(skill.dbName + " skill.script is null");
                return;
            }
            this.skills.Add(skill);
            if (skill.script != null)
            {
                SkillScript[] script = skill.script;
                foreach (SkillScript skillScript in script)
                {
                    if (!this.scripts.ContainsKey(skillScript.triggerType))
                    {
                        this.scripts[skillScript.triggerType] = new List<SkillScript>();
                    }
                    this.scripts[skillScript.triggerType].Add(skillScript);
                }
            }
            if (!string.IsNullOrEmpty(skill.onJoinWithUnit))
            {
                if (this.onJoinTriggers == null)
                {
                    this.onJoinTriggers = new List<Skill>();
                }
                this.onJoinTriggers.Add(skill);
            }
            if (!string.IsNullOrEmpty(skill.onLeaveFromUnit))
            {
                if (this.onLeaveTriggers == null)
                {
                    this.onLeaveTriggers = new List<Skill>();
                }
                this.onLeaveTriggers.Add(skill);
            }
        }

        public void Remove(Skill skill)
        {
            for (int i = 0; i < this.skills.Count; i++)
            {
                if (this.skills[i].Get() != skill)
                {
                    continue;
                }
                if (skill.script != null)
                {
                    SkillScript[] script = skill.script;
                    foreach (SkillScript skillScript in script)
                    {
                        if (this.scripts != null && this.scripts.ContainsKey(skillScript.triggerType) && this.scripts[skillScript.triggerType].Contains(skillScript))
                        {
                            this.scripts[skillScript.triggerType].Remove(skillScript);
                        }
                    }
                }
                this.skills.RemoveAt(i);
                break;
            }
            if (!string.IsNullOrEmpty(skill.onJoinWithUnit) && this.onJoinTriggers != null)
            {
                this.onJoinTriggers.Remove(skill);
                if (this.onJoinTriggers.Count == 0)
                {
                    this.onJoinTriggers = null;
                }
            }
            if (!string.IsNullOrEmpty(skill.onLeaveFromUnit) && this.onLeaveTriggers != null)
            {
                this.onLeaveTriggers.Remove(skill);
                if (this.onLeaveTriggers.Count == 0)
                {
                    this.onLeaveTriggers = null;
                }
            }
        }

        public List<SkillScript> GetSkillsByType(ESkillType eType, bool canReturnNull = true)
        {
            if (this.scripts == null || !this.scripts.ContainsKey(eType))
            {
                if (canReturnNull)
                {
                    return null;
                }
                return new List<SkillScript>();
            }
            return this.scripts[eType];
        }

        public Dictionary<ESkillType, List<SkillScript>> GetSkillScripts()
        {
            return this.scripts;
        }

        public List<DBReference<Skill>> GetSkills()
        {
            return this.skills;
        }

        public SkillManager CopySkillManager(ISkillable newOwner)
        {
            return new SkillManager
            {
                owner = newOwner,
                scripts = new Dictionary<ESkillType, List<SkillScript>>(this.scripts),
                skills = new List<DBReference<Skill>>(this.skills),
                onJoinTriggers = ((this.onJoinTriggers != null) ? new List<Skill>(this.onJoinTriggers) : null),
                onLeaveTriggers = ((this.onLeaveTriggers != null) ? new List<Skill>(this.onLeaveTriggers) : null)
            };
        }

        public void CopySkillManagerFrom(ISkillable source, List<Skill> exceptions = null)
        {
            foreach (DBReference<Skill> s in source.GetSkillManager().GetSkills())
            {
                if (exceptions == null || exceptions.Find((Skill o) => o == s) == null)
                {
                    this.owner.AddSkill(s);
                }
            }
        }

        public void CopySkillManagerFrom(SkillManager source, List<Skill> exceptions = null)
        {
            foreach (DBReference<Skill> s in source.GetSkills())
            {
                if (exceptions == null || exceptions.Find((Skill o) => o == s) == null)
                {
                    this.owner.AddSkill(s);
                }
            }
        }

        private void EnsureScriptDictionary()
        {
            if (this.scripts != null)
            {
                return;
            }
            this.scripts = new Dictionary<ESkillType, List<SkillScript>>();
            foreach (DBReference<Skill> skill in this.skills)
            {
                if (skill.Get().script == null)
                {
                    continue;
                }
                SkillScript[] script = skill.Get().script;
                foreach (SkillScript skillScript in script)
                {
                    if (!this.scripts.ContainsKey(skillScript.triggerType))
                    {
                        this.scripts[skillScript.triggerType] = new List<SkillScript>();
                    }
                    this.scripts[skillScript.triggerType].Add(skillScript);
                }
            }
        }

        private void EnsureJoinLeaveLists()
        {
            foreach (DBReference<Skill> skill in this.skills)
            {
                if (!string.IsNullOrEmpty(skill.Get().onJoinWithUnit))
                {
                    if (this.onJoinTriggers == null)
                    {
                        this.onJoinTriggers = new List<Skill>();
                    }
                    this.onJoinTriggers.Add(skill.Get());
                }
                if (!string.IsNullOrEmpty(skill.Get().onLeaveFromUnit))
                {
                    if (this.onLeaveTriggers == null)
                    {
                        this.onLeaveTriggers = new List<Skill>();
                    }
                    this.onLeaveTriggers.Add(skill.Get());
                }
            }
        }

        public void OnJoinTriggers(ISkillable otherSkillable, IEnumerable unitList)
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
            foreach (Skill onJoinTrigger in this.onJoinTriggers)
            {
                ScriptLibrary.Call(onJoinTrigger.onJoinWithUnit, this.owner, otherSkillable, onJoinTrigger, list);
            }
        }

        public void OnLeaveTriggers(ISkillable otherSkillable, IEnumerable unitList)
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
            foreach (Skill onLeaveTrigger in this.onLeaveTriggers)
            {
                ScriptLibrary.Call(onLeaveTrigger.onLeaveFromUnit, this.owner, otherSkillable, onLeaveTrigger, list);
            }
        }

        public void TriggerScripts(ESkillType eType, object data, ISkillable customTarget = null)
        {
            if (this.scripts == null || !this.scripts.ContainsKey(eType))
            {
                return;
            }
            if (customTarget == null)
            {
                _ = this.owner;
            }
            ISkillable skillable = this.owner;
            if (skillable.GetSkills() == null)
            {
                return;
            }
            foreach (DBReference<Skill> skill in skillable.GetSkills())
            {
                if (skill.Get().script == null)
                {
                    continue;
                }
                SkillScript[] script = skill.Get().script;
                foreach (SkillScript skillScript in script)
                {
                    if (skillScript.triggerType == eType && (string.IsNullOrEmpty(skillScript.trigger) || (bool)ScriptLibrary.Call(skillScript.trigger, skillable, skill.Get(), skillScript)))
                    {
                        ScriptLibrary.Call(skillScript.activatorMain, skillable, skill.Get(), skillScript);
                    }
                }
            }
        }

        public void ResetSkillManager()
        {
            this.scripts = new Dictionary<ESkillType, List<SkillScript>>();
            this.onJoinTriggers = new List<Skill>();
            this.onLeaveTriggers = new List<Skill>();
            this.skills = new List<DBReference<Skill>>();
        }
    }
}
