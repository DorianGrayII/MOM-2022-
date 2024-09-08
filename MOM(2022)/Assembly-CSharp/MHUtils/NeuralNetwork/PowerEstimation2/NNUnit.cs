namespace MHUtils.NeuralNetwork.PowerEstimation2
{
    using DBDef;
    using DBEnum;
    using MHUtils;
    using MHUtils.NeuralNetwork;
    using MOM;
    using ProtoBuf;
    using System;
    using System.Collections.Generic;

    [ProtoContract]
    public class NNUnit : INeuralData
    {
        public static List<TAG> unitParamsPairs;
        public static List<TAG> unitParams;
        public static List<string> unitSkills;
        [ProtoMember(1)]
        public double[] attributes;
        [ProtoMember(2)]
        public double targetValue;

        static NNUnit()
        {
            List<TAG> list1 = new List<TAG>();
            list1.Add(TAG.MELEE_ATTACK);
            list1.Add(TAG.MELEE_ATTACK_CHANCE);
            list1.Add(TAG.RANGE_ATTACK);
            list1.Add(TAG.RANGE_ATTACK_CHANCE);
            list1.Add(TAG.DEFENCE);
            list1.Add(TAG.DEFENCE_CHANCE);
            unitParamsPairs = list1;
            List<TAG> list2 = new List<TAG>();
            list2.Add(TAG.RESIST);
            list2.Add(TAG.RESIST_ELEMENTS);
            list2.Add(TAG.HIT_POINTS);
            list2.Add(TAG.WEAPON_IMMUNITY);
            list2.Add(TAG.MISSILE_IMMUNITY);
            list2.Add(TAG.NON_CORPOREAL);
            list2.Add(TAG.LONG_RANGE);
            list2.Add(TAG.MAGIC_RANGE);
            list2.Add(TAG.CAN_FLY);
            list2.Add(TAG.MOVEMENT_POINTS);
            list2.Add(TAG.LARGE_SHIELD);
            unitParams = list2;
            List<string> list3 = new List<string>();
            list3.Add("FIRE_BREATH");
            list3.Add("THROWN");
            list3.Add("DEATH_GAZE");
            list3.Add("DOOM_GAZE");
            list3.Add("POISON");
            list3.Add("LIFE_STEALING");
            list3.Add("IMMOLATION");
            list3.Add("ARMOR_PIERCING");
            list3.Add("ILLUSION");
            list3.Add("FIRST_STRIKE");
            unitSkills = list3;
        }

        public NNUnit(BattleUnit bu, int targetValue)
        {
            List<double> list = new List<double>();
            for (int i = 0; i < unitParamsPairs.Count; i += 2)
            {
                FInt attFinal = IAttributeableExtension.GetAttFinal(bu, unitParamsPairs[i]);
                FInt num4 = IAttributeableExtension.GetAttFinal(bu, unitParamsPairs[i + 1]);
                list.Add((double) (attFinal.ToFloat() * num4.ToFloat()));
            }
            for (int j = 0; j < unitParams.Count; j++)
            {
                FInt attFinal = IAttributeableExtension.GetAttFinal(bu, unitParams[j]);
                list.Add((double) attFinal.ToFloat());
            }
            int count = list.Count;
            for (int k = 0; k < unitSkills.Count; k++)
            {
                list.Add(0.0);
            }
            int num8 = 0;
            while (num8 < ISkillableExtension.GetSkills(bu).Count)
            {
                DBReference<Skill> reference = ISkillableExtension.GetSkills(bu)[num8];
                int num9 = 0;
                while (true)
                {
                    if (num9 >= unitSkills.Count)
                    {
                        num8++;
                        break;
                    }
                    if (reference.Get().dbName.Contains(unitSkills[num9]) && (reference.Get().script != null))
                    {
                        foreach (SkillScript script in reference.Get().script)
                        {
                            if ((script.triggerType == ESkillType.BattleAttackAddon) || (script.triggerType == ESkillType.BattleAttackAddon2))
                            {
                                list[count + num9] = script.fIntParam.ToFloat();
                            }
                            if (script.triggerType == ESkillType.BattleStackModifier)
                            {
                                list[count + num9] = 1.0;
                            }
                        }
                    }
                    num9++;
                }
            }
            list.Add((double) bu.GetMaxFigureCount());
            this.attributes = list.ToArray();
            this.targetValue = ((double) targetValue) / 10000.0;
        }

        public double[] GetData()
        {
            return this.attributes;
        }

        public static int GetInputDataSize()
        {
            return ((((unitParamsPairs.Count / 2) + unitParams.Count) + unitSkills.Count) + 1);
        }

        public bool Identical(NNUnit a, NNUnit b)
        {
            for (int i = 0; i < this.attributes.Length; i++)
            {
                if (a.attributes[i] != b.attributes[i])
                {
                    return false;
                }
            }
            return true;
        }

        public bool Identical(NNUnit a, BattleUnit b)
        {
            NNUnit unit1 = new NNUnit(b, 0);
            return this.Identical(a, b);
        }
    }
}

