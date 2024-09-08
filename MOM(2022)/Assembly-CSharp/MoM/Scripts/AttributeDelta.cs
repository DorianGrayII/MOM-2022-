namespace MoM.Scripts
{
    using DBDef;
    using MHUtils;
    using MOM;
    using ProtoBuf;
    using System;
    using System.Collections.Generic;

    [ProtoContract]
    public class AttributeDelta
    {
        [ProtoMember(1)]
        public NetDictionary<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>> originalAttributeSets;
        public NetDictionary<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>> currentAttributeSets;
        public NetDictionary<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>> deltaAttributeSets;

        public AttributeDelta()
        {
            this.originalAttributeSets = new NetDictionary<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>>();
            this.currentAttributeSets = new NetDictionary<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>>();
            this.deltaAttributeSets = new NetDictionary<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>>();
        }

        public AttributeDelta(IAttributable a)
        {
            this.originalAttributeSets = new NetDictionary<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>>();
            this.currentAttributeSets = new NetDictionary<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>>();
            this.deltaAttributeSets = new NetDictionary<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>>();
            this.originalAttributeSets.Clear();
            this.GetCurrentAttributes(a, this.originalAttributeSets);
        }

        public void CalcDeltas(IAttributable a)
        {
            this.deltaAttributeSets.Clear();
            this.GetCurrentAttributes(a, this.currentAttributeSets);
            foreach (KeyValuePair<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>> pair in this.currentAttributeSets)
            {
                NetDictionary<DBReference<Tag>, FInt> dictionary = pair.Value;
                NetDictionary<DBReference<Tag>, FInt> dictionary2 = this.originalAttributeSets[pair.Key];
                NetDictionary<DBReference<Tag>, FInt> dictionary3 = new NetDictionary<DBReference<Tag>, FInt>();
                foreach (KeyValuePair<DBReference<Tag>, FInt> pair2 in dictionary2)
                {
                    FInt num = pair2.Value;
                    FInt num2 = dictionary[pair2.Key];
                    dictionary3[pair2.Key] = num2 - num;
                }
                foreach (KeyValuePair<DBReference<Tag>, FInt> pair3 in dictionary)
                {
                    if (!dictionary2.ContainsKey(pair3.Key))
                    {
                        dictionary3[pair3.Key] = pair3.Value;
                    }
                }
                this.deltaAttributeSets[pair.Key] = dictionary3;
            }
            foreach (KeyValuePair<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>> pair4 in this.originalAttributeSets)
            {
                if (!this.currentAttributeSets.ContainsKey(pair4.Key))
                {
                    NetDictionary<DBReference<Tag>, FInt> dictionary4 = new NetDictionary<DBReference<Tag>, FInt>();
                    foreach (KeyValuePair<DBReference<Tag>, FInt> pair5 in pair4.Value)
                    {
                        dictionary4[pair5.Key] = FInt.ZERO - pair5.Value;
                    }
                    this.deltaAttributeSets[pair4.Key] = dictionary4;
                }
            }
        }

        private void GetCurrentAttributes(IAttributable a, NetDictionary<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>> skillDeltas)
        {
            if (a is ISkillable)
            {
                ISkillable skillable = a as ISkillable;
                if (ISkillableExtension.GetSkills(skillable) != null)
                {
                    using (List<DBReference<Skill>>.Enumerator enumerator = ISkillableExtension.GetSkills(skillable).GetEnumerator())
                    {
                        Skill skill;
                        SkillScript[] script;
                        int num;
                        goto TR_0010;
                    TR_0002:
                        num++;
                    TR_000B:
                        while (true)
                        {
                            if (num >= script.Length)
                            {
                                break;
                            }
                            SkillScript script = script[num];
                            if (script.triggerType == ESkillType.AttributeChange)
                            {
                                DBReference<Skill> reference;
                                NetDictionary<DBReference<Tag>, FInt> dictionary = new NetDictionary<DBReference<Tag>, FInt>();
                                if (!string.IsNullOrEmpty(script.trigger))
                                {
                                    object[] objArray1 = new object[5];
                                    objArray1[0] = skillable;
                                    objArray1[2] = reference.Get();
                                    objArray1[3] = script;
                                    objArray1[4] = dictionary;
                                    if (!((bool) ScriptLibrary.Call(script.trigger, objArray1)))
                                    {
                                        goto TR_0002;
                                    }
                                }
                                object[] parameters = new object[] { skillable, reference.Get(), script, dictionary };
                                ScriptLibrary.Call(script.activatorMain, parameters);
                                skillDeltas[skill] = dictionary;
                            }
                            goto TR_0002;
                        }
                    TR_0010:
                        while (true)
                        {
                            if (!enumerator.MoveNext())
                            {
                                break;
                            }
                            skill = enumerator.Current.Get();
                            if (skill.script != null)
                            {
                                script = skill.script;
                                num = 0;
                                goto TR_000B;
                            }
                        }
                    }
                }
            }
        }
    }
}

