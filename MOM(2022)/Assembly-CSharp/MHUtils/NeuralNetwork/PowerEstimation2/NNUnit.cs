using System.Collections.Generic;
using DBDef;
using DBEnum;
using MOM;
using ProtoBuf;

namespace MHUtils.NeuralNetwork.PowerEstimation2
{
    [ProtoContract]
    public class NNUnit : INeuralData
    {
        public static List<TAG> unitParamsPairs = new List<TAG>
        {
            TAG.MELEE_ATTACK,
            TAG.MELEE_ATTACK_CHANCE,
            TAG.RANGE_ATTACK,
            TAG.RANGE_ATTACK_CHANCE,
            TAG.DEFENCE,
            TAG.DEFENCE_CHANCE
        };

        public static List<TAG> unitParams = new List<TAG>
        {
            TAG.RESIST,
            TAG.RESIST_ELEMENTS,
            TAG.HIT_POINTS,
            TAG.WEAPON_IMMUNITY,
            TAG.MISSILE_IMMUNITY,
            TAG.NON_CORPOREAL,
            TAG.LONG_RANGE,
            TAG.MAGIC_RANGE,
            TAG.CAN_FLY,
            TAG.MOVEMENT_POINTS,
            TAG.LARGE_SHIELD
        };

        public static List<string> unitSkills = new List<string> { "FIRE_BREATH", "THROWN", "DEATH_GAZE", "DOOM_GAZE", "POISON", "LIFE_STEALING", "IMMOLATION", "ARMOR_PIERCING", "ILLUSION", "FIRST_STRIKE" };

        [ProtoMember(1)]
        public double[] attributes;

        [ProtoMember(2)]
        public double targetValue;

        public NNUnit(BattleUnit bu, int targetValue)
        {
            List<double> list = new List<double>();
            for (int i = 0; i < NNUnit.unitParamsPairs.Count; i += 2)
            {
                FInt attFinal = bu.GetAttFinal(NNUnit.unitParamsPairs[i]);
                FInt attFinal2 = bu.GetAttFinal(NNUnit.unitParamsPairs[i + 1]);
                list.Add(attFinal.ToFloat() * attFinal2.ToFloat());
            }
            for (int j = 0; j < NNUnit.unitParams.Count; j++)
            {
                list.Add(bu.GetAttFinal(NNUnit.unitParams[j]).ToFloat());
            }
            int count = list.Count;
            for (int k = 0; k < NNUnit.unitSkills.Count; k++)
            {
                list.Add(0.0);
            }
            for (int l = 0; l < bu.GetSkills().Count; l++)
            {
                DBReference<Skill> dBReference = bu.GetSkills()[l];
                for (int m = 0; m < NNUnit.unitSkills.Count; m++)
                {
                    if (!dBReference.Get().dbName.Contains(NNUnit.unitSkills[m]) || dBReference.Get().script == null)
                    {
                        continue;
                    }
                    SkillScript[] script = dBReference.Get().script;
                    foreach (SkillScript skillScript in script)
                    {
                        if (skillScript.triggerType == ESkillType.BattleAttackAddon || skillScript.triggerType == ESkillType.BattleAttackAddon2)
                        {
                            list[count + m] = skillScript.fIntParam.ToFloat();
                        }
                        if (skillScript.triggerType == ESkillType.BattleStackModifier)
                        {
                            list[count + m] = 1.0;
                        }
                    }
                }
            }
            list.Add(bu.GetMaxFigureCount());
            this.attributes = list.ToArray();
            this.targetValue = (double)targetValue / 10000.0;
        }

        public bool Identical(NNUnit a, BattleUnit b)
        {
            new NNUnit(b, 0);
            return this.Identical(a, b);
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

        public static int GetInputDataSize()
        {
            return NNUnit.unitParamsPairs.Count / 2 + NNUnit.unitParams.Count + NNUnit.unitSkills.Count + 1;
        }

        public double[] GetData()
        {
            return this.attributes;
        }
    }
}
