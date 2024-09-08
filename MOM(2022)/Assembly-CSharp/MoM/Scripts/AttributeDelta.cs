using System.Collections.Generic;
using DBDef;
using MHUtils;
using MOM;
using ProtoBuf;

namespace MoM.Scripts
{
    [ProtoContract]
    public class AttributeDelta
    {
        [ProtoMember(1)]
        public NetDictionary<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>> originalAttributeSets = new NetDictionary<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>>();

        public NetDictionary<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>> currentAttributeSets = new NetDictionary<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>>();

        public NetDictionary<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>> deltaAttributeSets = new NetDictionary<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>>();

        public AttributeDelta()
        {
        }

        public AttributeDelta(IAttributable a)
        {
            this.originalAttributeSets.Clear();
            this.GetCurrentAttributes(a, this.originalAttributeSets);
        }

        public void CalcDeltas(IAttributable a)
        {
            this.deltaAttributeSets.Clear();
            this.GetCurrentAttributes(a, this.currentAttributeSets);
            foreach (KeyValuePair<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>> currentAttributeSet in this.currentAttributeSets)
            {
                NetDictionary<DBReference<Tag>, FInt> value = currentAttributeSet.Value;
                NetDictionary<DBReference<Tag>, FInt> netDictionary = this.originalAttributeSets[currentAttributeSet.Key];
                NetDictionary<DBReference<Tag>, FInt> netDictionary2 = new NetDictionary<DBReference<Tag>, FInt>();
                foreach (KeyValuePair<DBReference<Tag>, FInt> item in netDictionary)
                {
                    FInt value2 = item.Value;
                    FInt fInt = value[item.Key];
                    netDictionary2[item.Key] = fInt - value2;
                }
                foreach (KeyValuePair<DBReference<Tag>, FInt> item2 in value)
                {
                    if (!netDictionary.ContainsKey(item2.Key))
                    {
                        netDictionary2[item2.Key] = item2.Value;
                    }
                }
                this.deltaAttributeSets[currentAttributeSet.Key] = netDictionary2;
            }
            foreach (KeyValuePair<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>> originalAttributeSet in this.originalAttributeSets)
            {
                if (this.currentAttributeSets.ContainsKey(originalAttributeSet.Key))
                {
                    continue;
                }
                NetDictionary<DBReference<Tag>, FInt> value3 = originalAttributeSet.Value;
                NetDictionary<DBReference<Tag>, FInt> netDictionary3 = new NetDictionary<DBReference<Tag>, FInt>();
                foreach (KeyValuePair<DBReference<Tag>, FInt> item3 in value3)
                {
                    netDictionary3[item3.Key] = FInt.ZERO - item3.Value;
                }
                this.deltaAttributeSets[originalAttributeSet.Key] = netDictionary3;
            }
        }

        private void GetCurrentAttributes(IAttributable a, NetDictionary<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>> skillDeltas)
        {
            if (!(a is ISkillable))
            {
                return;
            }
            ISkillable skillable = a as ISkillable;
            if (skillable.GetSkills() == null)
            {
                return;
            }
            foreach (DBReference<Skill> skill2 in skillable.GetSkills())
            {
                Skill skill = skill2.Get();
                if (skill.script == null)
                {
                    continue;
                }
                SkillScript[] script = skill.script;
                foreach (SkillScript skillScript in script)
                {
                    if (skillScript.triggerType == ESkillType.AttributeChange)
                    {
                        NetDictionary<DBReference<Tag>, FInt> netDictionary = new NetDictionary<DBReference<Tag>, FInt>();
                        if (string.IsNullOrEmpty(skillScript.trigger) || (bool)ScriptLibrary.Call(skillScript.trigger, skillable, null, skill2.Get(), skillScript, netDictionary))
                        {
                            ScriptLibrary.Call(skillScript.activatorMain, skillable, skill2.Get(), skillScript, netDictionary);
                            skillDeltas[skill] = netDictionary;
                        }
                    }
                }
            }
        }
    }
}
